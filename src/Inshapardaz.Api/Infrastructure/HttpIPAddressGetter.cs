using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Infrastructure
{
    public class HttpIPAddressGetter : IGetIPAddress
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public HttpIPAddressGetter(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string GetIPAddressFromRequest()
        {
            if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                return _contextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];
            else
                return _contextAccessor.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }
    }
}
