AspNetCoreRateLimitRegex
==============

The original author of this project is Stefan Prodan (https://github.com/stefanprodan) at the project AspNetCoreRateLimit (https://github.com/stefanprodan/AspNetCoreRateLimit) and all the credit for the original code and concept goes to him.

This version of the project features Regular Expressions Based Rule Configuration as well as a URL Based Rate Limiting Middleware that can be used to throttle requests based on the Url instead of the Caller or the Requests.

AspNetCoreRateLimitRegex is an ASP.NET Core rate limiting solution designed to control the rate of requests that clients can make to a Web API or MVC app based on the callers IP address or the requests client ID or the URL being called.
The [AspNetCoreRateLimitRegex package](https://www.nuget.org/packages/AspNetCoreRateLimitRegex/) contains an IpRateLimitMiddleware, a ClientRateLimitMiddleware and a UrlRateLimitMiddleware.
With each middleware you can set multiple limits for different scenarios like allowing an IP or Client to make a maximum number of calls in a time interval like per second, 15 minutes, etc or allowing a Url to be called a set number of times regardless of the caller with UrlRateLimitMiddleware.
You can define these limits to address all requests made to an API or you can scope the limits to each API URL or HTTP verb and path.
