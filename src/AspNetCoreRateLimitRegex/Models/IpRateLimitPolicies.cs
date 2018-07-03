using System.Collections.Generic;

namespace AspNetCoreRateLimitRegex
{
    public class IpRateLimitPolicies
    {
        public List<IpRateLimitPolicy> IpRules { get; set; }
    }
}
