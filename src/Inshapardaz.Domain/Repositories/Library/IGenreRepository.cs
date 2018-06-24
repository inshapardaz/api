using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IGenreRepository
    {
        Task<Genre> AddGenre(Genre genre, CancellationToken cancellationToken);

        Task UpdateGenre(Genre genre, CancellationToken cancellationToken);

        Task DeleteGenre(int genereId, CancellationToken cancellationToken);

        Task<IEnumerable<Genre>> GetGenres(CancellationToken cancellationToken);
        
        Task<Genre> GetGenreById(int genereId, CancellationToken cancellationToken);
    }
}