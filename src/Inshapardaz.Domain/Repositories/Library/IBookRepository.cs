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

        Task<Page<Book>> GetBooksByGenere(int genereId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Page<Book>> GetBooksByAuthor(int authorId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        
        Task<Book> GetBookById(int bookId, CancellationToken cancellationToken);
    }
}