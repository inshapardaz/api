using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Inshapardaz.Api.Middlewares
{
    public class TestAuthenticatedRequestMiddleware
    {
        public const string TestingCookieAuthentication = "TestCookieAuthentication";
        public const string TestingHeader = "X-Integration-Testing";
        public const string TestingHeaderValue = "inshapardaz-integraion-test";

        private readonly RequestDelegate _next;

        public TestAuthenticatedRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.Keys.Contains(TestingHeader) &&
                context.Request.Headers[TestingHeader].First().Equals(TestingHeaderValue))
            {
                if (context.Request.Headers.Keys.Contains("my-name"))
                {
                    var name = context.Request.Headers["my-name"].First();
                    var id = context.Request.Headers.Keys.Contains("my-id")
                        ? context.Request.Headers["my-id"].First()
                        : "";
                    var claimsIdentity = new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, name),
                        new Claim(NameIdentifierClaimName, id),

                    }, TestingCookieAuthentication);

                    if (context.Request.Headers.ContainsKey("reader"))
                        claimsIdentity.AddClaim(new Claim(AuthDataClaimName, AuthHeaderForRole("reader")));
                    if (context.Request.Headers.ContainsKey("admin"))
                        claimsIdentity.AddClaim(new Claim(AuthDataClaimName, AuthHeaderForRole("admin")));
                    if (context.Request.Headers.ContainsKey("writer"))
                        claimsIdentity.AddClaim(new Claim(AuthDataClaimName, AuthHeaderForRole("writer")));
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    context.User = claimsPrincipal;
                }
            }
            await _next(context);
        }

        private const string AuthDataClaimName = "https://api.inshapardaz.org/user_authorization";
        private const string NameIdentifierClaimName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        private string AuthHeaderForRole(string role) => JsonConvert.SerializeObject(new UserAuthenticationData { Roles = new[] { role } });

        private class UserAuthenticationData
        {
            public IEnumerable<string> Groups { get; set; }
            public IEnumerable<string> Roles { get; set; }
            public IEnumerable<string> Permissions { get; set; }
        }
    }
}
