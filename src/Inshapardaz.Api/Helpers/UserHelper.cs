using System.Security.Claims;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Helpers;

public class UserHelper(IHttpContextAccessor contextAccessor)
    : IUserHelper
{
    private ClaimsPrincipal User => contextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public bool IsAdmin => IsAuthenticated && IsSuperAdmin;

    public bool IsDictionaryWriter => IsAuthenticated && HasDictionaryWriterClaim;

    public bool IsEditor => IsAuthenticated && (IsSuperAdmin || HasDictionaryWriterClaim || HasWriterRoleInAnyLibrary);

    public bool IsLibraryAdmin(int libraryId) =>
        IsAuthenticated && (IsAdmin || HasLibraryRole(libraryId, Role.LibraryAdmin));

    public bool IsWriter(int libraryId) =>
        IsAuthenticated && (IsLibraryAdmin(libraryId) || HasLibraryRole(libraryId, Role.Writer));

    public AccountModel Account
    {
        get
        {
            if (!IsAuthenticated) return null;

            var idClaim = User.FindFirst("id")?.Value;
            if (idClaim == null || !int.TryParse(idClaim, out var id)) return null;

            return new AccountModel
            {
                Id = id,
                Name = User.FindFirst(ClaimTypes.Name)?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                IsSuperAdmin = IsSuperAdmin,
                IsDictionaryWriter = HasDictionaryWriterClaim
            };
        }
    }

    private bool IsSuperAdmin =>
        bool.TryParse(User?.FindFirst("isSuperAdmin")?.Value, out var isSuperAdmin) && isSuperAdmin;

    private bool HasDictionaryWriterClaim =>
        bool.TryParse(User?.FindFirst("isDictionaryWriter")?.Value, out var isDictionaryWriter) && isDictionaryWriter;

    private bool HasWriterRoleInAnyLibrary =>
        User?.Claims.Any(c => c.Type.StartsWith("lib:") && c.Type.EndsWith(":role")
            && Enum.TryParse<Role>(c.Value, out var role)
            && role is Role.Admin or Role.LibraryAdmin or Role.Writer) ?? false;

    private bool HasLibraryRole(int libraryId, Role role)
    {
        if (IsSuperAdmin) return true;

        var roleClaim = User?.FindFirst($"lib:{libraryId}:role")?.Value;
        if (roleClaim == null) return false;

        return Enum.TryParse<Role>(roleClaim, out var claimRole) && claimRole == role;
    }
}
