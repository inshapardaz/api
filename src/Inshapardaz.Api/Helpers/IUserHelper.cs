using Inshapardaz.Api.Entities;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Helpers
{
    public interface IUserHelper
    {
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }

        bool IsLibraryAdmin(int libraryId);

        bool IsWriter(int libraryId);

        bool IsReader(int libraryId);

        AccountModel Account { get; }
    }
}
