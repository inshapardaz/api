using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Inshapardaz.Functions.Tests.Helpers
{
    public static class AuthenticationBuilder
    {
        private const string AdminRole = "admin";
        private const string WriteRole = "writer";
        private const string ReaderRole = "reader";
        private const string IdentityClaimsName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        public static ClaimsPrincipal AdminClaim => CreateClaimsPrincipalWithRole(AdminRole);

        public static ClaimsPrincipal WriterClaim => CreateClaimsPrincipalWithRole(WriteRole);

        public static ClaimsPrincipal ReaderClaim => CreateClaimsPrincipalWithRole(ReaderRole);

        public static ClaimsPrincipal Unauthorized => CreateUnauthorizedPrincipal();

        private static ClaimsPrincipal CreateUnauthorizedPrincipal()
        {
            return null;
        }

        private static ClaimsPrincipal CreateClaimsPrincipalWithRole(string role)
        {
            var authenticationData = new UserAuthenticationData
            {
                Groups = new string[0],
                Roles = new string[1] {role},
                Permissions = new string[0],
            };
            var claims = new List<Claim>
            {
                new Claim("https://api.inshapardaz.org/user_authorization", JsonConvert.SerializeObject(authenticationData)),
                new Claim(IdentityClaimsName, Guid.NewGuid().ToString("D"))
            };
            var identity = new ClaimsIdentity(new ClaimsIdentity(claims, "Basic"));
            return new ClaimsPrincipal(identity);
        }

        public static Guid GetUserId(this ClaimsPrincipal principal) => 
            Guid.Parse(principal.Claims.FirstOrDefault( c => c.Type == IdentityClaimsName)?.Value ?? Guid.Empty.ToString());
    }

    internal class UserAuthenticationData
    {
        public IEnumerable<string> Groups { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}
