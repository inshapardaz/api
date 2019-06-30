using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Inshapardaz.Functions.Authentication
{
    public static class AuthenticationHelper
    {
        public static bool IsAuthenticated(this ClaimsPrincipal principal) => principal.Identity.IsAuthenticated;

        public static bool IsAdministrator(this ClaimsPrincipal principal) 
            => IsAuthenticated(principal) && GetRoles(principal).Contains("admin");

        public static bool IsWriter(this ClaimsPrincipal principal) 
            => IsAuthenticated(principal) && (IsAdministrator(principal) || GetRoles(principal).Contains("writer"));

        public static bool IsReader(this ClaimsPrincipal principal)
            => IsAuthenticated(principal) && (IsWriter(principal) || GetRoles(principal).Contains("reader"));

        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var nameIdentifier = principal.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (nameIdentifier != null)
            {
                return Guid.Parse(nameIdentifier.Replace("auth0|", "00000000"));
            }
            
            return Guid.Empty;
        }

        private static IEnumerable<string> GetRoles(ClaimsPrincipal principal)

        {
            var rolesIdentifierValue = principal.Claims.FirstOrDefault(c => c.Type == "https://api.inshapardaz.org/user_authorization")?.Value;

            if (!string.IsNullOrWhiteSpace(rolesIdentifierValue))
            {
                var authData = JsonConvert.DeserializeObject<UserAuthenticationData>(rolesIdentifierValue);

                return authData.Roles;
            }

            return Enumerable.Empty<string>();
        }

        private class UserAuthenticationData
        {
            public IEnumerable<string> Groups { get; set; }
            public IEnumerable<string> Roles { get; set; }
            public IEnumerable<string> Permissions { get; set; }
        }
    }
}