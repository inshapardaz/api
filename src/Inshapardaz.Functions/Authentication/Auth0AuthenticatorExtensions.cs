using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Inshapardaz.Functions.Authentication
{
    public static class Auth0AuthenticatorExtensions
    {
        public static async Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)> AuthenticateAsync(this Auth0Authenticator @this, AuthenticationHeaderValue header,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"##### {header.Scheme} - {header.Parameter}");
            if (header == null || !string.Equals(header.Scheme, "Bearer", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException("Authentication header does not use Bearer token.");
            }
            return await @this.AuthenticateAsync(header.Parameter, cancellationToken);
        }

        public static Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)> AuthenticateAsync(this Auth0Authenticator @this, HttpRequest request,
            CancellationToken cancellationToken = new CancellationToken()) =>
            @this.AuthenticateAsync(AuthenticationHeaderValue.Parse(request.Headers["Authorization"]), cancellationToken);
    }
}