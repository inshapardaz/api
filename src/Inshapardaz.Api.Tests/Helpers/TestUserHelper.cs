using Inshapardaz.Api.Helpers;
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

        public bool IsAuthenticated => GetUserId() != Guid.Empty;
        public bool IsAdmin => IsAuthenticated && IsUserInRole("admin");
        public bool IsLibraryAdmin => IsAuthenticated && IsUserInRole("libraryadmin");
        public bool IsWriter => IsAuthenticated && (IsLibraryAdmin || IsUserInRole("writer"));
        public bool IsReader => IsAuthenticated;

        public ClaimsPrincipal Claims => new ClaimsPrincipal(new ClaimsIdentity(_claimsProvider.Claims, "Test"));

        public Guid GetUserId()
        {
            return _claimsProvider.Claims.Any() ? Guid.NewGuid() : Guid.Empty;
        }

        private bool IsUserInRole(string role)
        {
            return Claims.HasClaim("permissions", role);
        }
    }
}
