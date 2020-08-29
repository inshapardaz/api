using System;
using System.Security.Claims;

namespace Inshapardaz.Api.Helpers
{
    public interface IUserHelper
    {
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
        bool IsWriter { get; }
        bool IsReader { get; }
        ClaimsPrincipal Claims { get; }

        Guid GetUserId();
    }
}
