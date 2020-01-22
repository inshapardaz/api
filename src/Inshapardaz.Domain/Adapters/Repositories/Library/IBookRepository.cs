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
        Task<BookModel> AddBook(BookModel book, CancellationToken cancellationToken);

        Task UpdateBook(BookModel book, CancellationToken cancellationToken);

        Task DeleteBook(int bookId, CancellationToken cancellationToken);

        Task<Page<BookModel>> GetBooks(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<BookModel>> GetLatestBooks(CancellationToken cancellationToken);
        
        Task<Page<BookModel>> SearchBooks(string searchText, int pageNumber, int pageSize, CancellationToken cancellationToken);


        Task<Page<BookModel>> GetBooksByAuthor(int authorId, int pageNumber, int pageSize, CancellationToken cancellationToken);


        Task<Page<BookModel>> GetBooksByCategory(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<BookModel> GetBookById(int bookId, CancellationToken cancellationToken);


        Task AddRecentBook(Guid userId, int bookId, CancellationToken cancellationToken);

        Task DeleteBookFromRecent(Guid userId, int bookId, CancellationToken cancellationToken);

        Task<IEnumerable<BookModel>> GetRecentBooksByUser(Guid userId, int count, CancellationToken cancellationToken);

        Task AddBookFile(int bookId, int id, CancellationToken cancellationToken);

        Task<int> GetBookCountByAuthor(int authorId, CancellationToken cancellationToken);
        Task<Page<BookModel>> GetBooksBySeries(int seriesId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<int> GetBookCountBySeries(int seriesId, CancellationToken cancellationToken);
        Task<int> GetBookCountByCategory(int categoryId, CancellationToken cancellationToken);
        Task DeleteBookFile(int bookId, int fileId, CancellationToken cancellationToken);

        Task<IEnumerable<FileModel>> GetFilesByBook(int bookId, CancellationToken cancellationToken);

        Task<FileModel> GetBookFileById(int bookId, int fileId, CancellationToken cancellationToken);
    }
}
