using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Adapters;

public interface IUserHelper
{
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }

    // Account-wide (not per library): may manage the language tools -- spelling, meaning
    // dictionaries, transliteration and the autocorrect/punctuation lists.
    bool IsDictionaryWriter { get; }

    // Any account that writes somewhere: Admin, DictionaryWriter, or Writer/LibraryAdmin in at
    // least one library. Editors may maintain the shared autocorrect/punctuation lists.
    bool IsEditor { get; }

    bool CanManageLanguageTools => IsAdmin || IsDictionaryWriter;

    bool IsLibraryAdmin(int libraryId);

    bool IsWriter(int libraryId);

    AccountModel Account { get; }

    int? AccountId => Account?.Id;
}
