using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IAuthorRepository
    {
        Task<AuthorModel> AddAuthor(int libraryId, AuthorModel author, CancellationToken cancellationToken);

        Task UpdateAuthor(int libraryId, AuthorModel author, CancellationToken cancellationToken);

        Task UpdateAuthorImage(int libraryId, int authorId, int imageId, CancellationToken cancellationToken);

        Task DeleteAuthor(int libraryId, int authorId, CancellationToken cancellationToken);

        Task<Page<AuthorModel>> GetAuthors(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<AuthorModel> GetAuthorById(int libraryId, int authorId, CancellationToken cancellationToken);

        Task<Page<AuthorModel>> FindAuthors(int libraryId, string query, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<AuthorModel>> GetAuthorByIds(int libraryId, IEnumerable<int> authorIds, CancellationToken cancellationToken);
    }
}
