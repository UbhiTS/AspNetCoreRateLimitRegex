using System;

namespace AspNetCoreRateLimitRegex
{
    public class RateLimitRule
    {
        #region Public Members

        /// <summary>
        /// Url Regular Expression to Match
        /// </summary>
        public string UrlRegex { get; set; }

        /// <summary>
        /// Ignore individual matched path values
        /// </summary>
        public bool ProtectUrlUnmatched { get; set; }

        /// <summary>
        /// Rate limit period as in 1s, 1m, 1h
        /// </summary>
        public string Period { get; set; }

        /// <summary>
        /// Maximum number of requests that a client can make in the defined period
        /// </summary>
        public long Limit { get; set; }

        #endregion

        #region Private Members

        internal TimeSpan? PeriodTimespan { get; set; }

        #endregion
    }
}
