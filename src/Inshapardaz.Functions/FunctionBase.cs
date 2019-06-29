using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Inshapardaz.Functions
{
    public abstract class FunctionBase
    {
        protected async Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)> Authenticate(HttpRequestMessage req, ILogger log) 
        {
            return await req.AuthenticateAsync(log);
        }

        protected async Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)?> TryAuthenticate(HttpRequestMessage req, ILogger log) 
        {
            try
            {
                return await req.AuthenticateAsync(log);
            }
            catch(AuthenticationExpectedException)
            {
                return null;
            }
        }

        protected static LinkView SelfLink(string href, string relType = RelTypes.Self) => new LinkView { 
            Method = "GET", 
            Rel = relType, 
            Href = href
        };
    }
}