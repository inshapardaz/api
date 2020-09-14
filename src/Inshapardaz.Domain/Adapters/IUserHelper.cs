using System;
using System.ComponentModel;
using System.Security.Claims;

namespace Inshapardaz.Domain.Adapters
{
    public interface IUserHelper
    {
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
        bool IsLibraryAdmin { get; }
        bool IsWriter { get; }
        bool IsReader { get; }
        ClaimsPrincipal Claims { get; }

        Guid GetUserId();

        bool CheckPermissions(Permission[] permissions);
    }

    public enum Permission
    {
        [Description("unauthorised")]
        Unauthorised,

        [Description("reader")]
        Reader,

        [Description("writer")]
        Writer,

        [Description("libraryadmin")]
        LibraryAdmin,

        [Description("admin")]
        Admin
    }
}
