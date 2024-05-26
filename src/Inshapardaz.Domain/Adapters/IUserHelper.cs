using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Adapters;

public interface IUserHelper
{
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }

    bool IsLibraryAdmin(int libraryId);

    bool IsWriter(int libraryId);

    AccountModel Account { get; }

    int? AccountId => Account?.Id;
}
