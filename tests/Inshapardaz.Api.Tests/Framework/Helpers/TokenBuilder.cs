using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inshapardaz.Api.Tests.Framework.Helpers
{
    public static class TokenBuilder
    {
        public static string GenerateToken(Settings settings, int accountId)
        {
            return GenerateToken(settings, accountId, isSuperAdmin: false);
        }

        public static string GenerateToken(Settings settings, int accountId, 
            bool isSuperAdmin = false, 
            string name = null, 
            string email = null,
            int? libraryId = null, 
            Role? role = null)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.Security.Secret));

            var claims = new List<Claim>
            {
                new("id", accountId.ToString()),
                new(ClaimTypes.Name, name ?? string.Empty),
                new(ClaimTypes.Email, email ?? string.Empty),
                new("isSuperAdmin", isSuperAdmin.ToString())
            };

            if (libraryId.HasValue && role.HasValue)
            {
                claims.Add(new Claim($"lib:{libraryId.Value}:role", role.Value.ToString()));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "Inshapardaz",
                Audience = "Inshapardaz",
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
