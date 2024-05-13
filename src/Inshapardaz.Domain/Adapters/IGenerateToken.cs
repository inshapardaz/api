using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Adapters;

public interface IGenerateToken
{
    string GenerateAccessToken(AccountModel account);

    RefreshTokenModel GenerateRefreshToken(string ipAddress);

    string GenerateResetToken();
}
