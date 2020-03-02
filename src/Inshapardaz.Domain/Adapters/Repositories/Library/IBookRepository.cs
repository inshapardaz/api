using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IBookRepository
    {
        Task<BookModel> AddBook(int libraryId, BookModel book, Guid? userId, CancellationToken cancellationToken);

        Task UpdateBook(int libraryId, BookModel book, CancellationToken cancellationToken);

        Task DeleteBook(int libraryId, int bookId, CancellationToken cancellationToken);

        Task<Page<BookModel>> GetBooks(int libraryId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken);

        Task<Page<BookModel>> GetLatestBooks(int libraryId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken);

        Task<Page<BookModel>> SearchBooks(int libraryId, string searchText, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken);

        Task<Page<BookModel>> GetBooksByAuthor(int libraryId, int authorId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken);

        Task<Page<BookModel>> GetBooksByCategory(int libraryId, int categoryId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken);

        Task<Page<BookModel>> GetFavoriteBooksByUser(int libraryId, Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<BookModel> GetBookById(int libraryId, int bookId, Guid? userId, CancellationToken cancellationToken);

        Task AddRecentBook(int libraryId, Guid userId, int bookId, CancellationToken cancellationToken);

        Task DeleteBookFromRecent(int libraryId, Guid userId, int bookId, CancellationToken cancellationToken);

        Task<IEnumerable<BookModel>> GetRecentBooksByUser(int libraryId, Guid userId, int count, CancellationToken cancellationToken);

        Task AddBookToFavorites(Guid userId, int bookId, CancellationToken cancellationToken);

        Task DeleteBookFromFavorites(Guid userId, int bookId, CancellationToken cancellationToken);

        Task AddBookFile(int bookId, int id, CancellationToken cancellationToken);

        Task<Page<BookModel>> GetBooksBySeries(int libraryId, int seriesId, int pageNumber, int pageSize, Guid? userId, CancellationToken cancellationToken);

        Task DeleteBookFile(int bookId, int fileId, CancellationToken cancellationToken);

        Task<IEnumerable<FileModel>> GetFilesByBook(int bookId, CancellationToken cancellationToken);

        Task<FileModel> GetBookFileById(int bookId, int fileId, CancellationToken cancellationToken);
    }
}
