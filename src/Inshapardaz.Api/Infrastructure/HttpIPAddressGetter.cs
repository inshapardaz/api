using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Infrastructure;

public class HttpIPAddressGetter(IHttpContextAccessor contextAccessor) : IGetIPAddress
{
    public string GetIPAddressFromRequest()
    {
        if (contextAccessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            return contextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];
        else
            return contextAccessor.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }
}
