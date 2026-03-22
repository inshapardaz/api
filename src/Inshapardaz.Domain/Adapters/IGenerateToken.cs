using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Adapters;

public interface IGenerateToken
{
    string GenerateAccessToken(AccountModel account, IEnumerable<LibraryModel> libraries = null);

    RefreshTokenModel GenerateRefreshToken(string ipAddress);

    string GenerateResetToken();
}
