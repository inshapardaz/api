using Inshapardaz.Domain.Models.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library
{
    public interface ILibraryRepository
    {
        Task<LibraryModel> GetLibraryById(int libraryId, CancellationToken cancellationToken);
    }
}
