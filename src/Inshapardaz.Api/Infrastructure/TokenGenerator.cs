using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inshapardaz.Api.Infrastructure;


public class TokenGenerator(IOptions<Settings> settings) : IGenerateToken
{
    public const string Issuer = "Inshapardaz";
    public const string Audience = "Inshapardaz";

    private readonly Settings _settings = settings.Value;

    public string GenerateAccessToken(AccountModel account, IEnumerable<LibraryModel> libraries = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.Security.Secret);

        var claims = new List<Claim>
        {
            new("id", account.Id.ToString()),
            new(ClaimTypes.Name, account.Name ?? string.Empty),
            new(ClaimTypes.Email, account.Email ?? string.Empty),
            new("isSuperAdmin", account.IsSuperAdmin.ToString())
        };

        if (libraries != null)
        {
            foreach (var library in libraries)
            {
                claims.Add(new Claim($"lib:{library.Id}:role", library.Role.ToString()));
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_settings.Security.AccessTokenTTLInMinutes),
            Issuer = Issuer,
            Audience = Audience,
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

    public string GenerateResetToken() => RandomGenerator.GenerateRandomString();
}
