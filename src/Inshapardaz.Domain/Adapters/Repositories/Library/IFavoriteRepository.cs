using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IFavoriteRepository
    {
        Task AddBookToFavorites(Guid userId, int bookId, CancellationToken cancellationToken);

        Task DeleteBookFromFavorites(Guid userId, int bookId, CancellationToken cancellationToken);

        Task<Page<BookModel>> GetFavoriteBooksByUser(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<bool> IsBookFavorite(Guid userId, int bookId, CancellationToken cancellationToken);
    }
}
