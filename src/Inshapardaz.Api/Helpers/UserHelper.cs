using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILibraryRepository _libraryRepository;

        public UserHelper(IHttpContextAccessor contextAccessor, ILibraryRepository libraryRepository)
        {
            _contextAccessor = contextAccessor;
            _libraryRepository = libraryRepository;
        }

        public bool IsAuthenticated => Account != null;

        public bool IsAdmin => IsAuthenticated && IsUserInRole(Role.Admin);

        public bool IsLibraryAdmin(int libraryId) => IsAuthenticated && (IsAdmin || IsUserInRole(Role.LibraryAdmin, libraryId));

        public bool IsWriter(int libraryId) => IsAuthenticated && (IsLibraryAdmin(libraryId) || IsUserInRole(Role.Writer, libraryId));

        public bool IsReader(int libraryId) => IsAuthenticated && (IsWriter(libraryId) || IsUserInRole(Role.Reader, libraryId));

        public AccountModel Account => (AccountModel)_contextAccessor.HttpContext.Items["Account"];

        public bool IsUserInRole(Role role, int? libraryId = null)
        {
            var account = (AccountModel)_contextAccessor.HttpContext.Items["Account"];
            if (role == Role.Admin && account.IsSuperAdmin) return true;

            var libraries = _libraryRepository.GetUserLibraries(account.Id, 1, 100, CancellationToken.None).Result;
            return libraries.Data.Any(l => l.Id == libraryId && l.Role == role);
        }
    }
}
