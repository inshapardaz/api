using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library
{
    public interface ILibraryRepository
    {
        Task<Page<LibraryModel>> GetLibraries(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Page<LibraryModel>> FindLibraries(string query, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Page<LibraryModel>> GetUserLibraries(int accountId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Page<LibraryModel>> FindUserLibraries(string query, int accountId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Page<LibraryModel>> GetUnassignedLibraries(int accountId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Page<LibraryModel>> FindUnassignedLibraries(string query, int accountId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<LibraryModel> GetLibraryById(int libraryId, CancellationToken cancellationToken);

        Task<LibraryModel> AddLibrary(LibraryModel library, CancellationToken cancellationToken);

        Task UpdateLibrary(LibraryModel library, CancellationToken cancellationToken);

        Task DeleteLibrary(int libraryId, CancellationToken cancellationToken);

        Task AddAccountToLibrary(int libraryId, int accountId, Role role, CancellationToken cancellationToken);

        Task UpdateLibraryUser(LibraryUserModel model, CancellationToken cancellationToken);

        Task RemoveLibraryFromAccount(int libraryId, int accountId, CancellationToken cancellationToken);

        Task<IEnumerable<LibraryModel>> GetLibrariesByAccountId(int accountId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
