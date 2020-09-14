using Inshapardaz.Api.Extensions;
using Inshapardaz.Domain.Adapters;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace Inshapardaz.Api.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public bool IsAuthenticated => GetUserId() != Guid.Empty;

        public bool IsAdmin => IsAuthenticated && IsUserInRole("admin");
        public bool IsLibraryAdmin => IsAuthenticated && IsUserInRole("libraryAdmin");

        public bool IsWriter => IsAuthenticated && (IsLibraryAdmin || IsUserInRole("writer"));

        public bool IsReader => IsAuthenticated;

        public ClaimsPrincipal Claims => _contextAccessor.HttpContext.User;

        public bool CheckPermissions(Permission[] permissions)
        {
            return permissions.Any(p => IsUserInRole(p));
        }

        public Guid GetUserId()
        {
            var nameIdentifier = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (nameIdentifier != null)
            {
                return Guid.Parse(nameIdentifier.Replace("auth0|", "00000000"));
            }

            return Guid.Empty;
        }

        private bool IsUserInRole(string role)
        {
            return Claims.HasClaim("permissions", role);
        }

        private bool IsUserInRole(Permission permission) => IsUserInRole(permission.ToDescription());
    }
}
