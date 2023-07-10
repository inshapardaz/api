using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IBookRepository
    {
        Task<BookModel> AddBook(int libraryId, BookModel book, int? accountId, CancellationToken cancellationToken);

        Task UpdateBook(int libraryId, BookModel book, CancellationToken cancellationToken);

        Task DeleteBook(int libraryId, int bookId, CancellationToken cancellationToken);

        Task<Page<BookModel>> GetBooks(int libraryId, int pageNumber, int pageSize, int? accountId, BookFilter filter, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken);

        Task<Page<BookModel>> SearchBooks(int libraryId, string searchText, int pageNumber, int pageSize, int? accountId, BookFilter filter, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken);

        Task<BookModel> GetBookById(int libraryId, int bookId, int? accountId, CancellationToken cancellationToken);

        Task<BookModel> GetBookBySource(int libraryId, string source, CancellationToken cancellationToken);

        Task AddRecentBook(int libraryId, int accountId, int bookId, CancellationToken cancellationToken);

        Task DeleteBookFromRecent(int libraryId, int accountId, int bookId, CancellationToken cancellationToken);

        Task AddBookToFavorites(int libraryId, int? accountId, int bookId, CancellationToken cancellationToken);
        Task<Page<BookModel>> GetBooksByUser(int libraryId, int accountId, int pageNumber, int pageSize, StatusType status, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken);
        Task DeleteBookFromFavorites(int libraryId, int accountId, int bookId, CancellationToken cancellationToken);

        Task AddBookContent(int bookId, int fileId, string language, string mimeType, CancellationToken cancellationToken);

        Task DeleteBookContent(int libraryId, int bookId, string language, string mimeType, CancellationToken cancellationToken);

        Task<BookContentModel> GetBookContent(int libraryId, int bookId, string language, string mimeType, CancellationToken cancellationToken);

        Task<IEnumerable<BookContentModel>> GetBookContents(int libraryId, int bookId, CancellationToken cancellationToken);

        Task UpdateBookImage(int libraryId, int bookId, int fileId, CancellationToken cancellationToken);

        Task UpdateBookContentUrl(int libraryId, int bookId, string language, string mimeType, string url, CancellationToken cancellationToken);

        Task<IEnumerable<BookPageSummaryModel>> GetBookPageSummary(int libraryId, IEnumerable<int> bookIds, CancellationToken cancellationToken);
    }
}
