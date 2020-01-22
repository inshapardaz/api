using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IAuthorRepository
    {
        Task<AuthorModel> AddAuthor(AuthorModel author, CancellationToken cancellationToken);

        Task UpdateAuthor(AuthorModel author, CancellationToken cancellationToken);

        Task DeleteAuthor(int authorId, CancellationToken cancellationToken);

        Task<Page<AuthorModel>> GetAuthors(int pageNumber, int pageSize, CancellationToken cancellationToken);
        
        Task<AuthorModel> GetAuthorById(int authorId, CancellationToken cancellationToken);

        Task<Page<AuthorModel>> FindAuthors(string query, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}