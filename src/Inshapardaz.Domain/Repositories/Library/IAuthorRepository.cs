using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IAuthorRepository
    {
        Task<Author> AddAuthor(Author author, CancellationToken cancellationToken);

        Task UpdateAuthor(Author author, CancellationToken cancellationToken);

        Task DeleteAuthor(int authorId, CancellationToken cancellationToken);

        Task<Page<Author>> GetAuthors(int pageNumber, int pageSize, CancellationToken cancellationToken);
        
        Task<Author> GetAuthorById(int authorId, CancellationToken cancellationToken);

        Task<Page<Author>> FindAuthors(string query, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}