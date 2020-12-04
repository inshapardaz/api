using Inshapardaz.Api.Entities;

namespace Inshapardaz.Api.Helpers
{
    public interface IUserHelper
    {
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
        bool IsLibraryAdmin { get; }
        bool IsWriter { get; }
        bool IsReader { get; }

        Account Account { get; }
    }
}
