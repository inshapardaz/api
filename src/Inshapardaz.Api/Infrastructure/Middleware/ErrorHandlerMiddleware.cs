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

            // Only the app's own domain exceptions carry a message that's safe to hand back
            // to the client -- they're thrown with fixed, app-authored strings (e.g. "Book
            // does not exist"). Anything else (default case) could be a raw DB error, file
            // path, or stack detail, so the client gets a generic message and the real detail
            // only goes to the log.
            string clientMessage;

            switch (error)
            {
                case UnauthorizedException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    clientMessage = error.Message;
                    break;
                case UnauthorizedAccessException:
                    // Framework exception (e.g. can be thrown by file IO with a real path in
                    // the message), never thrown by app code with an intentional message here.
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    clientMessage = "Unauthorized.";
                    break;
                case ForbiddenException:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    clientMessage = error.Message;
                    break;
                case ConflictException:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    clientMessage = error.Message;
                    break;
                case NotFoundException:
                    // A NotFoundException on a non-GET request maps to 400 rather than 404 --
                    // e.g. "delete a thing that doesn't exist" is treated as a bad request,
                    // not a missing resource, matching the app's existing convention.
                    response.StatusCode = context.Request.Method == HttpMethods.Get
                        ? (int)HttpStatusCode.NotFound
                        : (int)HttpStatusCode.BadRequest;
                    clientMessage = error.Message;
                    break;
                case KeyNotFoundException:
                    // Framework exception, not app-authored -- keep the same status mapping
                    // as NotFoundException but never forward its message.
                    response.StatusCode = context.Request.Method == HttpMethods.Get
                        ? (int)HttpStatusCode.NotFound
                        : (int)HttpStatusCode.BadRequest;
                    clientMessage = "Not found.";
                    break;
                case BadRequestException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    clientMessage = error.Message;
                    break;
                case NotImplementedException:
                    response.StatusCode = (int)HttpStatusCode.NotImplemented;
                    clientMessage = error.Message;
                    break;
                default:
                    logger.LogError(error, "Unhandled error processing request");
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    clientMessage = "An unexpected error occurred.";
                    break;
            }

            var result = JsonSerializer.Serialize(new { message = clientMessage });
            await response.WriteAsync(result);
        }
    }
}
