using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AspNetCoreRateLimitRegex
{
    public class UrlRateLimitProcessor
    {
        private readonly UrlRateLimitOptions _options;
        private readonly IRateLimitCounterStore _counterStore;
        private readonly RateLimitCore _core;

        private static readonly object _processLocker = new object();

        public UrlRateLimitProcessor(UrlRateLimitOptions options, IRateLimitCounterStore counterStore)
        {
            _options = options;
            _counterStore = counterStore;

            _core = new RateLimitCore(Processor.UrlRateLimit, options, _counterStore);
        }

        public List<RateLimitRule> GetMatchingRules(ClientRequestIdentity identity)
        {
            var limits = new List<RateLimitRule>();

            // search for matching general rules
            if (_options.GeneralRules != null)
            {
                var matchingGeneralLimits = new List<RateLimitRule>();

                foreach (var generalRule in _options.GeneralRules)
                {
                    var regex = new Regex(generalRule.UrlRegex, RegexOptions.IgnoreCase);
                    var match = regex.Match(identity.HttpVerb + ":" + identity.Path);
                    if (match.Success)
                    {
                        matchingGeneralLimits.Add(generalRule);
                    }
                }

                // get the most restrictive general limit for each period 
                limits = matchingGeneralLimits.GroupBy(l => l.Period).Select(l => l.OrderBy(x => x.Limit)).Select(l => l.First()).ToList();
            }

            foreach (var item in limits)
            {
                //parse period text into time spans
                item.PeriodTimespan = _core.ConvertToTimeSpan(item.Period);
            }

            limits = limits.OrderBy(l => l.PeriodTimespan).ToList();

            if(_options.StackBlockedRequests)
            {
                limits.Reverse();   
            }

            return limits;
        }

        public bool IsWhitelisted(ClientRequestIdentity requestIdentity)
        {
            if (_options.ClientWhitelist != null && _options.ClientWhitelist.Contains(requestIdentity.ClientId))
            {
                return true;
            }

            if (_options.UrlWhitelist != null && _options.UrlWhitelist.Any())
            {
                if (_options.UrlWhitelist.Any(x => $"{requestIdentity.HttpVerb}:{requestIdentity.Path}".ContainsIgnoreCase(x)) ||
                    _options.UrlWhitelist.Any(x => $"*:{requestIdentity.Path}".ContainsIgnoreCase(x)))
                    return true;
            }

            return false;
        }

        public RateLimitCounter ProcessRequest(ClientRequestIdentity requestIdentity, RateLimitRule rule)
        {
            return _core.ProcessRequest(requestIdentity, rule);
        }

        public RateLimitHeaders GetRateLimitHeaders(ClientRequestIdentity requestIdentity, RateLimitRule rule)
        {
            return _core.GetRateLimitHeaders(requestIdentity, rule);
        }

        public string RetryAfterFrom(DateTime timestamp, RateLimitRule rule)
        {
            return _core.RetryAfterFrom(timestamp, rule);
        }
    }
}
