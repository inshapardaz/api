using Inshapardaz.Domain.Exception;

namespace Inshapardaz.Api.Infrastructure.Middleware;

// Many endpoints are anonymous-friendly: they never enforce [Authorize] and instead check
// User.Identity.IsAuthenticated to decide whether to return extra/owned data. That means a
// supplied token that fails validation (expired, malformed, wrong signature, ...) is never
// challenged by the framework -- it just leaves the user unauthenticated, so the request is
// silently served as an anonymous call instead of failing. JwtBearerEvents.OnAuthenticationFailed
// (see Program.cs) flags the failure on HttpContext.Items; this middleware turns that flag into
// an explicit 401 so a caller that did send a token always gets a definitive answer.
public class TokenValidationMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        if (context.Items.ContainsKey("Bearer.AuthenticationFailed"))
        {
            throw new UnauthorizedException();
        }

        await next(context);
    }
}
