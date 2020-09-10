using Inshapardaz.Domain.Models.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library
{
    public interface ILibraryRepository
    {
        Task<LibraryModel> GetLibraryById(int libraryId, CancellationToken cancellationToken);

        Task<LibraryModel> AddLibrary(LibraryModel library, CancellationToken cancellationToken);

        Task UpdateLibrary(LibraryModel library, CancellationToken cancellationToken);

        Task DeleteLibrary(int libraryId, CancellationToken cancellationToken);
    }
}
