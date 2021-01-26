using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
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

        Task<LibraryModel> GetLibraryById(int libraryId, CancellationToken cancellationToken);

        Task<LibraryModel> AddLibrary(LibraryModel library, CancellationToken cancellationToken);

        Task UpdateLibrary(LibraryModel library, CancellationToken cancellationToken);

        Task DeleteLibrary(int libraryId, CancellationToken cancellationToken);

        Task AddLibraryToAccount(int libraryId, int accountId, CancellationToken cancellationToken);

        Task RemoveLibraryFromAccount(int libraryId, int accountId, CancellationToken cancellationToken);
    }
}
