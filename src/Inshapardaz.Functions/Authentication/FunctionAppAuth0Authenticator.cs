using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Inshapardaz.Functions.Authentication
{
    public interface IFunctionAppAuthenticator
    {
        Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)> AuthenticateAsync(HttpRequestMessage request, ILogger log);
    }

    public class FunctionAppAuth0Authenticator : IFunctionAppAuthenticator
    {
        private static readonly Lazy<Auth0Authenticator> Authenticator = new Lazy<Auth0Authenticator>(() => new Auth0Authenticator(ConfigurationSettings.Auth0Domain, new [] { ConfigurationSettings.Audience }));

        public async Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)> AuthenticateAsync(HttpRequestMessage request, ILogger log)
        {
            var authenticator = Authenticator.Value;
            try
            {
                return await authenticator.AuthenticateAsync(request);
            }
            catch (Exception ex)
            {
                log.LogError("Authorization failed", ex);
                throw new AuthenticationExpectedException();
            }
        }
    }
}