using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Inshapardaz.Functions.Authentication
{
    public class AccessTokenValueProvider : IValueProvider
    {
        private const string AUTH_HEADER_NAME = "Authorization";
        private const string BEARER_PREFIX = "Bearer ";
        private readonly HttpRequest _request;
        private readonly string _audience;
        private readonly string _issuer;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _manager;

        public AccessTokenValueProvider(HttpRequest request, string audience, string issuer)
        {
            _request = request;
            _audience = audience;
            _issuer = issuer;

            _manager = new ConfigurationManager<OpenIdConnectConfiguration>($"https://{_issuer}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
        }

        public async Task<object> GetValueAsync()
        {
            if(_request.Headers.ContainsKey(AUTH_HEADER_NAME) && 
               _request.Headers[AUTH_HEADER_NAME].ToString().StartsWith(BEARER_PREFIX))
            {
                var token = _request.Headers["Authorization"].ToString().Substring(BEARER_PREFIX.Length);
                var config = await _manager.GetConfigurationAsync().ConfigureAwait(false);

                var parameters = new TokenValidationParameters
                {
                    ValidIssuer = $"https://{_issuer}/",
                    ValidAudiences = new[] {_audience},
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = config.SigningKeys
                };

                var handler = new JwtSecurityTokenHandler();
                return handler.ValidateToken(token, parameters, out var securityToken);
            }

            return null;
            //throw new SecurityTokenException("No access token submitted.");
        }

        public Type Type => typeof(ClaimsPrincipal);

        public string ToInvokeString() => string.Empty;
    }
}