using System.Security.Claims;

namespace Inshapardaz.Domain.Adapters
{
    public interface IReadClaims
    {
        bool IsWriter(ClaimsPrincipal claims);

        bool IsAdministrator(ClaimsPrincipal claims);
    }
}
