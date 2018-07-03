using Microsoft.AspNetCore.Builder;

namespace AspNetCoreRateLimitRegex
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseIpRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IpRateLimitMiddleware>();
        }

        public static IApplicationBuilder UseClientRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ClientRateLimitMiddleware>();
        }

        public static IApplicationBuilder UseUrlRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UrlRateLimitMiddleware>();
        }
    }
}
