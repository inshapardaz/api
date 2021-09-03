using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using System.Threading;

namespace Inshapardaz.Api.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Settings _appSettings;

        public JwtMiddleware(RequestDelegate next, Settings appSettings)
        {
            _next = next;
            _appSettings = appSettings;
        }

        public async Task Invoke(HttpContext context, ILibraryRepository libraryRepository, IAccountRepository accountRepository)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachAccountToContext(context, token, libraryRepository, accountRepository);

            await _next(context);
        }

        private async Task AttachAccountToContext(HttpContext context, string token, ILibraryRepository libraryRepository, IAccountRepository accountRepository)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                context.Items["Account"] = await accountRepository.GetAccountById(accountId, CancellationToken.None);
                context.Items["Libraries"] = await libraryRepository.GetLibrariesByAccountId(accountId, CancellationToken.None);
            }
            catch
            {
            }
        }
    }
}
