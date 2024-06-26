﻿using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inshapardaz.Api.Infrastructure;


public class TokenGenerator : IGenerateToken
{
    private readonly Settings _settings;

    public TokenGenerator(IOptions<Settings> settings)
    {
        _settings = settings.Value;
    }

    public string GenerateAccessToken(AccountModel account)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.Security.Secret);

        var claims = new List<Claim>() { new Claim("id", account.Id.ToString()) };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_settings.Security.AccessTokenTTLInMinutes),
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
            Expires = DateTime.UtcNow.AddDays(_settings.Security.RefreshTokenTTLInDays),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
    }

    public string GenerateResetToken()
    {
        return RandomGenerator.GenerateRandomString();
    }
}
