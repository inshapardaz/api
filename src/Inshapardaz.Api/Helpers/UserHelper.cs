using Inshapardaz.Domain.Adapters;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using Inshapardaz.Api.Entities;

namespace Inshapardaz.Api.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public bool IsAuthenticated => GetUserId() != null;

        public bool IsAdmin => IsAuthenticated && IsUserInRole(Role.Admin);
        public bool IsLibraryAdmin => IsAuthenticated && IsUserInRole(Role.LibraryAdmin);

        public bool IsWriter => IsAuthenticated && (IsLibraryAdmin || IsUserInRole(Role.Writer));

        public bool IsReader => IsAuthenticated;

        public ClaimsPrincipal Claims => _contextAccessor.HttpContext.User;

        public int? GetUserId()
        {
            var account = (Account)_contextAccessor.HttpContext.Items["Account"];
            return account?.Id;
        }

        private bool IsUserInRole(Role role)
        {
            var account = (Account)_contextAccessor.HttpContext.Items["Account"];
            return account.Role == role;
        }
    }
}
