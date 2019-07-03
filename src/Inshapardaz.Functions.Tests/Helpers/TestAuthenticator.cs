using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Inshapardaz.Functions.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Tests
{
    public static class AuthenticationBuilder
    {
        public static IFunctionAppAuthenticator  AsAdmin()
        {
            return new TestAuthenticator(TestAuthenticator.AdminRole);
        }

        public static IFunctionAppAuthenticator  AsWriter()
        {
            return new TestAuthenticator(TestAuthenticator.AdminRole);
        }

        public static IFunctionAppAuthenticator  AsReader()
        {
            return new TestAuthenticator(TestAuthenticator.AdminRole);
        }
    }

    public class TestAuthenticator : IFunctionAppAuthenticator
    {
        public const string AdminRole = "admin";
        public const string WriteRole = "writer";
        public const string ReaderRole = "reader";
        private string _role = "";
    
        public TestAuthenticator(string role)
        {
            _role = role;    
        }

        public async Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)> AuthenticateAsync(HttpRequest request, ILogger log)
        {
            var authenticationData = new UserAuthenticationData{
                    Groups = new string[0],
                    Roles = new string[1] { _role },
                    Permissions = new string[0],
                };
            var claims = new List<Claim>{
                new Claim("https://api.inshapardaz.org/user_authorization",  JsonConvert.SerializeObject(authenticationData))
            };
            var identity = new ClaimsIdentity(new ClaimsIdentity(claims, "Basic"));
            var principal = new ClaimsPrincipal(identity);
            
            return await Task.FromResult((
                principal,
                (SecurityToken) null
            ));
        }
    }

    internal class UserAuthenticationData
    {
        public IEnumerable<string> Groups { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}