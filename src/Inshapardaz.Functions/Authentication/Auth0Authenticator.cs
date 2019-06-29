using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Inshapardaz.Functions.Authentication
{
    public class Auth0Authenticator
    {
        private readonly TokenValidationParameters _parameters;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _manager;
        private readonly JwtSecurityTokenHandler _handler;

        public Auth0Authenticator(string auth0Domain, IEnumerable<string> audiences)
        {
            _parameters = new TokenValidationParameters
            {
                ValidIssuer = $"https://{auth0Domain}/",
                ValidAudiences = audiences.ToArray(),
                ValidateIssuerSigningKey = true,
            };
            _manager = new ConfigurationManager<OpenIdConnectConfiguration>($"https://{auth0Domain}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
            _handler = new JwtSecurityTokenHandler();
        }

        public async Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)> AuthenticateAsync(string token, CancellationToken cancellationToken = new CancellationToken())
        {
            // Note: ConfigurationManager<T> has an automatic refresh interval of 1 day.
            //   The config is cached in-between refreshes, so this "asynchronous" call actually completes synchronously unless it needs to refresh.
            var config = await _manager.GetConfigurationAsync(cancellationToken).ConfigureAwait(false);
            _parameters.IssuerSigningKeys = config.SigningKeys;
            var user = _handler.ValidateToken(token, _parameters, out var validatedToken);
            return (user, validatedToken);
        }
    }
}