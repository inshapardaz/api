using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IBookRepository
    {
        Task<Book> AddBook(Book book, CancellationToken cancellationToken);

        Task UpdateBook(Book book, CancellationToken cancellationToken);

        Task DeleteBook(int bookId, CancellationToken cancellationToken);

        Task<Page<Book>> GetBooks(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<Book>> GtLatestBooks(CancellationToken cancellationToken);
        
        Task<Page<Book>> GetPublicBooks(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Page<Book>> SearchBooks(string searchText, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Page<Book>> SearchPublicBooks(string searchText, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Page<Book>> GetBooksByAuthor(int authorId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Page<Book>> GetPublicBooksByAuthor(int authorId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Page<Book>> GetBooksByCategory(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Page<Book>> GetPublicBooksByCategory(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Book> GetBookById(int bookId, CancellationToken cancellationToken);


        Task AddRecentBook(Guid userId, int bookId, CancellationToken cancellationToken);
        Task DeleteBookFromRecent(Guid userId, int bookId, CancellationToken cancellationToken);

        Task<IEnumerable<Book>> GetRecentBooksByUser(Guid userId, int count, CancellationToken cancellationToken);

        Task<Page<Book>> GetFavoriteBooksByUser(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        
    }
}