using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Domain.Helpers.Dictionaries;

// Dictionaries are public to read. Changing one takes an Admin, or its owner while the owner still
// holds the DictionaryWriter role (revoking the role revokes edit rights on their dictionaries).
public static class DictionaryPermissions
{
    public static bool CanWrite(IUserHelper userHelper, DictionaryModel dictionary) =>
        userHelper.IsAdmin
        || (userHelper.IsDictionaryWriter && dictionary.OwnerId.HasValue && dictionary.OwnerId == userHelper.AccountId);

    public static void EnsureCanWrite(IUserHelper userHelper, DictionaryModel dictionary)
    {
        if (!userHelper.IsAuthenticated) throw new UnauthorizedException();
        if (!CanWrite(userHelper, dictionary)) throw new ForbiddenException();
    }
}
