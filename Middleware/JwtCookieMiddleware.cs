using BrochureAPI.Interfaces;
using Microsoft.Net.Http.Headers;

namespace BrochureAPI.Middleware
{
    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request has a cookie with the JWT token
            if (context.Request.Cookies.TryGetValue("access_token", out var token))
            {
                // Add the token to the Authorization header
                if (!string.IsNullOrEmpty(token) && !context.Request.Headers.ContainsKey(HeaderNames.Authorization))
                {
                    context.Request.Headers.Append(HeaderNames.Authorization, $"Bearer {token}");
                }
            }

            // Continue processing the request
            await _next(context);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline
    public static class JwtCookieMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtCookieMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtCookieMiddleware>();
        }
    }
} 