using Inshapardaz.Domain.Adapters;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Inshapardaz.Api.Entities;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public bool IsAuthenticated => GetAccountId() != null;

        public bool IsAdmin => IsAuthenticated && IsUserInRole(Role.Admin);
        public bool IsLibraryAdmin => IsAuthenticated && IsUserInRole(Role.LibraryAdmin);

        public bool IsWriter => IsAuthenticated && (IsLibraryAdmin || IsUserInRole(Role.Writer));

        public bool IsReader => IsAuthenticated && (IsWriter || IsUserInRole(Role.Reader));

        public ClaimsPrincipal Claims => _contextAccessor.HttpContext.User;

        public int? GetAccountId()
        {
            var account = (Account)_contextAccessor.HttpContext.Items["Account"];
            return account?.Id;
        }

        public bool IsUserInRole(Role role)
        {
            var account = (Account)_contextAccessor.HttpContext.Items["Account"];
            return account.Role == role;
        }
    }
}
