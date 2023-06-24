using Inshapardaz.Domain.Exception;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<StatusCodeMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<StatusCodeMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                _logger.LogInformation($"BGN : {context.Request.Method} {context.Request.Path} ");
                await _next.Invoke(context);
                _logger.LogInformation($"FIN : {context.Request.Method} {context.Request.Path} - {context.Response.StatusCode}");
            }
            catch
            {
                _logger.LogInformation($"Err : {context.Request.Method} {context.Request.Path} - {context.Response.StatusCode}");
            }
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestLoggingMiddleware>();
            return app;
        }
    }
}
