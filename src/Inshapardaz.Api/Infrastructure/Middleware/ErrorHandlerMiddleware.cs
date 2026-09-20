using System.Net;
using System.Text.Json;
using Inshapardaz.Domain.Exception;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Api.Infrastructure.Middleware;

// Single global exception handler. Previously this was split across two
// nested middlewares (ErrorHandlerMiddleware + StatusCodeMiddleware) with
// different response shapes; StatusCodeMiddleware's catch-all swallowed
// everything from the controller layer down without rethrowing, so the
// outer ErrorHandlerMiddleware only ever saw exceptions thrown above it in
// the pipeline (e.g. in LibraryConfigurationMiddleware). Consolidated into
// one middleware with one consistent { "message": ... } JSON contract.
public class ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch (error)
            {
                case UnauthorizedException:
                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case ForbiddenException:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    break;
                case ConflictException:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;
                case NotFoundException:
                case KeyNotFoundException:
                    // A NotFoundException on a non-GET request maps to 400 rather than 404 --
                    // e.g. "delete a thing that doesn't exist" is treated as a bad request,
                    // not a missing resource, matching the app's existing convention.
                    response.StatusCode = context.Request.Method == HttpMethods.Get
                        ? (int)HttpStatusCode.NotFound
                        : (int)HttpStatusCode.BadRequest;
                    break;
                case BadRequestException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case NotImplementedException:
                    response.StatusCode = (int)HttpStatusCode.NotImplemented;
                    break;
                default:
                    logger.LogError(error, "Unhandled error processing request");
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(new { message = error?.Message });
            await response.WriteAsync(result);
        }
    }
}
