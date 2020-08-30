using Inshapardaz.Domain.Adapters;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
            return _contextAccessor.HttpContext.User.HasClaim("permissions", role);
        }

        //private IEnumerable<string> GetRoles()

        //{
        //    var rolesIdentifierValue = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "https://api.inshapardaz.org/user_authorization")?.Value;

        //    if (!string.IsNullOrWhiteSpace(rolesIdentifierValue))
        //    {
        //        var authData = JsonConvert.DeserializeObject<UserAuthenticationData>(rolesIdentifierValue);

        //        return authData.Roles;
        //    }

        //    return Enumerable.Empty<string>();
        //}

        private class UserAuthenticationData
        {
            public IEnumerable<string> Groups { get; set; }
            public IEnumerable<string> Roles { get; set; }
            public IEnumerable<string> Permissions { get; set; }
        }
    }
}
