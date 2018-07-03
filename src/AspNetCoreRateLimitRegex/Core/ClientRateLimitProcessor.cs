using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AspNetCoreRateLimitRegex
{
    public class ClientRateLimitProcessor
    {
        private readonly ClientRateLimitOptions _options;
        private readonly IRateLimitCounterStore _counterStore;
        private readonly IClientPolicyStore _policyStore;
        private readonly RateLimitCore _core;

        private static readonly object _processLocker = new object();

        public ClientRateLimitProcessor(ClientRateLimitOptions options,
           IRateLimitCounterStore counterStore,
           IClientPolicyStore policyStore)
        {
            _options = options;
            _counterStore = counterStore;
            _policyStore = policyStore;

            _core = new RateLimitCore(Processor.ClientRateLimit, options, _counterStore);
        }

        public List<RateLimitRule> GetMatchingRules(ClientRequestIdentity identity)
        {
            var limits = new List<RateLimitRule>();
            var policy = _policyStore.Get($"{_options.ClientPolicyPrefix}_{identity.ClientId}");

            if (policy != null)
            {
                foreach (var rule in policy.Rules)
                {
                    var regex = new Regex(rule.UrlRegex, RegexOptions.IgnoreCase);
                    var match = regex.Match(identity.HttpVerb + ":" + identity.Path);
                    if (match.Success)
                    {
                        limits.Add(rule);
                    }
                }
            }

            // get the most restrictive limit for each period 
            limits = limits.GroupBy(l => l.Period).Select(l => l.OrderBy(x => x.Limit)).Select(l => l.First()).ToList();

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
                var generalLimits = matchingGeneralLimits.GroupBy(l => l.Period).Select(l => l.OrderBy(x => x.Limit)).Select(l => l.First()).ToList();

                foreach (var generalLimit in generalLimits)
                {
                    // add general rule if no specific rule is declared for the specified period
                    if(!limits.Exists(l => l.Period == generalLimit.Period))
                    {
                        limits.Add(generalLimit);
                    }
                }
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

            foreach (var url in _options.UrlWhitelist)
            {
                var regex = new Regex(url, RegexOptions.IgnoreCase);
                var match = regex.Match(requestIdentity.HttpVerb + ":" + requestIdentity.Path);
                if (match.Success)
                {
                    return true;
                }
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
