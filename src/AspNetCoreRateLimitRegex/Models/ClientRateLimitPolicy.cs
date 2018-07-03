using System.Collections.Generic;

namespace AspNetCoreRateLimitRegex
{
    public class ClientRateLimitPolicy
    {
        public string ClientId { get; set; }
        public List<RateLimitRule> Rules { get; set; }
    }
}
