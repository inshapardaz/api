using Inshapardaz.Api.Extensions;
using Inshapardaz.Domain.Adapters;
using System;
using System.Linq;
using System.Security.Claims;

namespace Inshapardaz.Api.Tests.Helpers
{
    public class TestUserHelper : IUserHelper
    {
        private readonly TestClaimsProvider _claimsProvider;

        public TestUserHelper(TestClaimsProvider claimsProvider)
        {
            _claimsProvider = claimsProvider;
        }

        public bool IsAuthenticated => GetUserId() != null;
        public bool IsAdmin => IsAuthenticated && IsUserInRole("admin");
        public bool IsLibraryAdmin => IsAuthenticated && IsUserInRole("libraryadmin");
        public bool IsWriter => IsAuthenticated && (IsLibraryAdmin || IsUserInRole("writer"));
        public bool IsReader => IsAuthenticated;

        public ClaimsPrincipal Claims => new ClaimsPrincipal(new ClaimsIdentity(_claimsProvider.Claims, "Test"));

        public bool CheckPermissions(Permission[] permissions)
        {
            return permissions.Any(p => IsUserInRole(p.ToDescription()));
        }

        public int? GetUserId()
        {
            var nameIdentifier = _claimsProvider.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (nameIdentifier != null)
            {
                return int.Parse(nameIdentifier);
            }

            return null;
        }

        private bool IsUserInRole(string role)
        {
            return Claims.HasClaim("permissions", role);
        }
    }
}
