using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Api.Middlewares
{
    public class StatusCodeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<StatusCodeMiddleware> _logger;

        public StatusCodeMiddleware(RequestDelegate next, ILogger<StatusCodeMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (UnauthorizedException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            catch (NotFoundException)
            {
                if (context.Request.Method == "GET")
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (BadRequestException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            catch (NotImplementedException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request");
                var telemetryClient = new TelemetryClient();
                telemetryClient.TrackException(ex);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }

    public static class StatusCodeMiddlewareExtensions
    {
        public static IApplicationBuilder UseStatusCodeMiddleWare(this IApplicationBuilder app)
        {
            app.UseMiddleware<StatusCodeMiddleware>();
            return app;
        }
    }
}