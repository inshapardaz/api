using Inshapardaz.Domain.Exception;
using System.Net;

namespace Inshapardaz.Api.Middleware
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
            catch (ForbiddenException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            catch (ConflictException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
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
