using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IBookRepository
    {
        Task<BookModel> AddBook(int libraryId, BookModel book, Guid? userId, CancellationToken cancellationToken);

        Task UpdateBook(int libraryId, BookModel book, CancellationToken cancellationToken);

        Task DeleteBook(int libraryId, int bookId, CancellationToken cancellationToken);

        Task<Page<BookModel>> GetBooks(int libraryId, int pageNumber, int pageSize, Guid? userId, BookFilter filter, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken);

        Task<Page<BookModel>> GetLatestBooks(int libraryId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken);

        Task<Page<BookModel>> SearchBooks(int libraryId, string searchText, int pageNumber, int pageSize, Guid? userId, BookFilter filter, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken);

        //Task<Page<BookModel>> GetBooksByAuthor(int libraryId, int authorId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken);

        //Task<Page<BookModel>> GetBooksByCategory(int libraryId, int categoryId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken);

        //Task<Page<BookModel>> GetFavoriteBooksByUser(int libraryId, Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<BookModel> GetBookById(int libraryId, int bookId, Guid? userId, CancellationToken cancellationToken);

        Task AddRecentBook(int libraryId, Guid userId, int bookId, CancellationToken cancellationToken);

        Task DeleteBookFromRecent(int libraryId, Guid userId, int bookId, CancellationToken cancellationToken);

        Task<IEnumerable<BookModel>> GetRecentBooksByUser(int libraryId, Guid userId, int count, CancellationToken cancellationToken);

        Task AddBookToFavorites(int libraryId, Guid? userId, int bookId, CancellationToken cancellationToken);

        Task DeleteBookFromFavorites(int libraryId, Guid userId, int bookId, CancellationToken cancellationToken);

        Task AddBookContent(int bookId, int fileId, string language, string mimeType, CancellationToken cancellationToken);

        //Task<Page<BookModel>> GetBooksBySeries(int libraryId, int seriesId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken);

        Task DeleteBookContent(int libraryId, int bookId, string language, string mimeType, CancellationToken cancellationToken);

        Task<BookContentModel> GetBookContent(int libraryId, int bookId, string language, string mimeType, CancellationToken cancellationToken);

        Task<IEnumerable<BookContentModel>> GetBookContents(int libraryId, int bookId, CancellationToken cancellationToken);

        Task UpdateBookImage(int libraryId, int bookId, int fileId, CancellationToken cancellationToken);

        Task UpdateBookContentUrl(int libraryId, int bookId, string language, string mimeType, string url, CancellationToken cancellationToken);
    }
}
