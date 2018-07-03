﻿using System.Collections.Generic;

namespace AspNetCoreRateLimitRegex
{
    public class IpRateLimitPolicy
    {
        public string Ip { get; set; }
        public List<RateLimitRule> Rules { get; set; }
    }
}
