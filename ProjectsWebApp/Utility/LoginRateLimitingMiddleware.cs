using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ProjectsWebApp.Utility
{
    /// <summary>
    /// Very small, in-memory rate limiter for the login endpoint to mitigate
    /// high-volume attacks across many accounts from a single IP.
    /// </summary>
    public class LoginRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<LoginRateLimitingMiddleware> _logger;

        private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);
        private const int MaxRequestsPerWindow = 30; // per IP per minute for POST /Identity/Account/Login

        public LoginRateLimitingMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<LoginRateLimitingMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only throttle POSTs to the login page
            if (HttpMethods.IsPost(context.Request.Method) &&
                context.Request.Path.HasValue &&
                context.Request.Path.Value.Contains("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase))
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var cacheKey = $"login-rate:{ip}";

                var now = DateTimeOffset.UtcNow;
                var entry = _cache.Get<LoginRateLimitEntry>(cacheKey);
                if (entry == null || now - entry.WindowStart > Window)
                {
                    entry = new LoginRateLimitEntry
                    {
                        WindowStart = now,
                        Count = 0
                    };
                }

                entry.Count++;
                _cache.Set(cacheKey, entry, now + Window);

                if (entry.Count > MaxRequestsPerWindow)
                {
                    _logger.LogWarning("Login rate limit exceeded for IP {Ip}", ip);

                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Zu viele Anmeldeversuche von dieser IP-Adresse. Bitte versuche es in kurzer Zeit erneut.");
                    return;
                }
            }

            await _next(context);
        }

        private class LoginRateLimitEntry
        {
            public DateTimeOffset WindowStart { get; set; }
            public int Count { get; set; }
        }
    }
}
