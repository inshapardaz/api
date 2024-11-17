using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Inshapardaz.Domain.Adapters.Configuration;
using Microsoft.Extensions.Options;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Api.Infrastructure.Middleware;

public class CookieAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Settings _appSettings;

    public CookieAuthenticationMiddleware(RequestDelegate next, IOptions<Settings> appSettings)
    {
        _next = next;
        _appSettings = appSettings.Value;
    }

    public async Task Invoke(HttpContext context, IAccountRepository accountRepository)
    {
        if (context.Items["AccountId"] is null)
        {
            var token = context.Request.Cookies["token"];

            if (token != null)
            {
                await AttachAccountToContext(context, token, accountRepository);
            }
        }

        await _next(context);
    }

    private async Task AttachAccountToContext(HttpContext context, string token, IAccountRepository accountRepository)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Security.Secret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.FromMinutes(1),
            }, out SecurityToken validatedToken);

            var accessToken = (JwtSecurityToken)validatedToken;
            var accountId = int.Parse(accessToken.Claims.First(x => x.Type == "id").Value);

            context.Items["AccountId"] = accountId;
            context.Items["Account"] = await accountRepository.GetAccountById(accountId, CancellationToken.None);
        }
        catch (SecurityTokenValidationException)
        {
            throw new UnauthorizedAccessException("JWT token invalid or expired");
        }
    }
}
