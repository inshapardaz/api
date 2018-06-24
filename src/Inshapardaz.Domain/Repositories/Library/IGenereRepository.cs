using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IGenereRepository
    {
        Task<Genere> AddGenere(Genere genere, CancellationToken cancellationToken);

        Task UpdateGenere(Genere genere, CancellationToken cancellationToken);

        Task DeleteGenere(int genereId, CancellationToken cancellationToken);

        Task<IEnumerable<Genere>> GetGeneres(CancellationToken cancellationToken);
        
        Task<Genere> GetGenereById(int genereId, CancellationToken cancellationToken);
    }
}