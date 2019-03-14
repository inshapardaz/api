using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface ISeriesRepository
    {
        Task<Series> AddSeries(Series series, CancellationToken cancellationToken);

        Task UpdateSeries(Series series, CancellationToken cancellationToken);

        Task DeleteSeries(int seriesId, CancellationToken cancellationToken);

        Task<IEnumerable<Series>> GetSeries(CancellationToken cancellationToken);

        Task<Series> GetSeriesById(int seriesId, CancellationToken cancellationToken);
    }
}