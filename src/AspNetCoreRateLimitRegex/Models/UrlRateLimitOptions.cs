using System.Collections.Generic;

namespace AspNetCoreRateLimitRegex
{
    public class UrlRateLimitOptions : RateLimitCoreOptions
    {
        public new List<RateLimitRule> GeneralRules { get; set; }

        /// <summary>
        /// Gets or sets the policy prefix, used to compose the url policy cache key
        /// </summary>
        public string UrlPolicyPrefix { get; set; } = "urlp";
    }
}
