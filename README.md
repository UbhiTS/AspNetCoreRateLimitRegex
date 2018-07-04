AspNetCoreRateLimitRegex
==============

The original author of this project is Stefan Prodan (https://github.com/stefanprodan) at the project AspNetCoreRateLimit (https://github.com/stefanprodan/AspNetCoreRateLimit) and all the credit for the original code and concept goes to him.

This version of the project features **Regular Expressions Based Rule Configuration** as well as a **URL Based Rate Limiting Middleware** that can be used to throttle requests based on the Url instead of the Caller or the Requests.

AspNetCoreRateLimitRegex is an ASP.NET Core rate limiting solution designed to control the rate of requests that clients can make to a Web API or MVC app based on the callers IP address or the requests client ID or the URL being called.
The [AspNetCoreRateLimitRegex package](https://www.nuget.org/packages/AspNetCoreRateLimitRegex/) contains an IpRateLimitMiddleware, a ClientRateLimitMiddleware and a UrlRateLimitMiddleware.
With each middleware you can set multiple limits for different scenarios like allowing an IP or Client to make a maximum number of calls in a time interval like per second, 15 minutes, etc or allowing a Url to be called a set number of times regardless of the caller with UrlRateLimitMiddleware.
You can define these limits to address all requests made to an API or you can scope the limits to each API URL (Http verb and path).

The original documentation still holds true for most of the features except all Rule Endpoints (now called UrlRegex) as Regular Expressions

For example:

Instead of `"Endpoint":"*"` you now say `"UrlRegex":".*:.*"` where the format of url regex is `"httpverb:urlpath"` ... so the 1st `".*"` in `UrlRegex` matches any Http Verb (get,post,put,delete,patch) and the 2nd `".*"` will match any url path.
If for example you want to rate limit all gets to the API, you would set the rule's `"UrlRegex":"get:.*"` and for throttling all updates `"UrlRegex":"put:.*"`

In any configuration such as the 'ClientRateLimiting', 'IpRateLimiting' or the `UrlRateLimiting` in `appsettings.json`, you have an optional parameter in the rule called **`"ProtectUrlUnmatched"`** which is `false` by default. Example :-

```json
"UrlRateLimiting": {
	"GeneralRules": [
		{
			"UrlRegex": "get:/api/values/microsoft",
			"Period": "5m",
			"Limit": 10
		},
		{
			"UrlRegex": "get:/api/values/.*/Users/list",
			"ProtectUrlUnmatched": true,
			"Period": "5m",
			"Limit": 5
		}
	]
}
```

If the `ProtectUrlUnmatched` is true for a rule then all the matched paths will count under one Limit. E.g.
`get:/api/values/.*/Users/list` matches all the below calls and you will be able to call the API endpoint only 5 times in 5 minutes overall per Client / Ip / Url depending on the configuration section it is in.
```
1. get:/api/values/microsoft/Users/list
2. get:/api/values/google/Users/list
3. get:/api/values/apple/Users/list
4. get:/api/values/facebook/Users/list
5. get:/api/values/microsoft/Users/list
6. API Quota Exceeded Message ...
7. API Quota Exceeded Message ...
8. API Quota Exceeded Message ...
```

If the `ProtectUrlUnmatched` is false then all the matched paths will count under their own separate limits. E.g.
`get:/api/values/.*/Users/list` matches all the below API calls and each url path can be called 5 times in 5 minutes per Url Path per Client / Ip / Url depending on the configuration section it is in
```
1. get:/api/values/microsoft/Users/list
2. get:/api/values/microsoft/Users/list
3. get:/api/values/microsoft/Users/list
4. get:/api/values/microsoft/Users/list
5. get:/api/values/microsoft/Users/list
6. API Quota Exceeded Message ...
7. API Quota Exceeded Message ...
8. API Quota Exceeded Message ...	

1. get:/api/values/google/Users/list
2. get:/api/values/google/Users/list
3. get:/api/values/google/Users/list
4. get:/api/values/google/Users/list
5. get:/api/values/google/Users/list
6. API Quota Exceeded Message ...
7. API Quota Exceeded Message ...
8. API Quota Exceeded Message ...	

1. get:/api/values/apple/Users/list
2. get:/api/values/apple/Users/list
3. get:/api/values/apple/Users/list
4. get:/api/values/apple/Users/list
5. get:/api/values/apple/Users/list
6. API Quota Exceeded Message ...
7. API Quota Exceeded Message ...
8. API Quota Exceeded Message ...	
```