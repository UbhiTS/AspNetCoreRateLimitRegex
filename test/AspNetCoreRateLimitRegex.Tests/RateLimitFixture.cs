using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreRateLimitRegex.Tests
{
    public class RateLimitFixture<TStartup> : RateLimitFixtureBase<TStartup>
        where TStartup : class
    {
        public RateLimitFixture() : base("http://localhost:5000")
        {
        }
    }
}
