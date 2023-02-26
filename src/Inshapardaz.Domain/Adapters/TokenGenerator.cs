using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inshapardaz.Domain.Adapters
{
    public interface IGenerateToken
    {
        string GenerateAccessToken(AccountModel account);

        RefreshTokenModel GenerateRefreshToken(string ipAddress);

        string GenerateResetToken();
    }

    public class TokenGenerator : IGenerateToken
    {
        private readonly Settings _settings;

        public TokenGenerator(Settings settings)
        {
            _settings = settings;
        }

        public string GenerateAccessToken(AccountModel account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.Secret);

            var claims = new List<Claim>() { new Claim("id", account.Id.ToString()) };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenTTLInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public RefreshTokenModel GenerateRefreshToken(string ipAddress)
        {
            return new RefreshTokenModel
            {
                Token = RandomGenerator.GenerateRandomString(),
                Expires = DateTime.UtcNow.AddDays(_settings.RefreshTokenTTLInDays),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        public string GenerateResetToken()
        {
            return RandomGenerator.GenerateRandomString();
        }
    }
}
