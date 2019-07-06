using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Inshapardaz.Functions.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Inshapardaz.Functions.Tests.Helpers
{
    public static class AuthenticationBuilder
    {
        private const string AdminRole = "admin";
        private const string WriteRole = "writer";
        private const string ReaderRole = "reader";

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
                new Claim("https://api.inshapardaz.org/user_authorization", JsonConvert.SerializeObject(authenticationData))
            };
            var identity = new ClaimsIdentity(new ClaimsIdentity(claims, "Basic"));
            return new ClaimsPrincipal(identity);
        }
    }

    internal class UserAuthenticationData
    {
        public IEnumerable<string> Groups { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}