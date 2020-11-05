using Inshapardaz.Domain.Models.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library
{
    public interface IBookPageRepository
    {
        Task<BookPageModel> GetPageByPageNumber(int libraryId, int bookId, int pageNumber, CancellationToken cancellationToken);

        Task<BookPageModel> AddPage(int libraryId, int bookId, int pageNumber, string text, int imageId, CancellationToken cancellationToken);

        Task<BookPageModel> UpdatePage(int libraryId, int bookId, int pageNumber, string text, int imageId, CancellationToken cancellationToken);

        Task DeletePage(int libraryId, int bookId, int pageNumber, CancellationToken cancellationToken);

        Task<BookPageModel> UpdatePageImage(int libraryId, int bookId, int pageNumber, int imageId, CancellationToken cancellationToken);
    }
}
