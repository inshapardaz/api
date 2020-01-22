using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface ISeriesRepository
    {
        Task<SeriesModel> AddSeries(SeriesModel series, CancellationToken cancellationToken);

        Task UpdateSeries(SeriesModel series, CancellationToken cancellationToken);

        Task DeleteSeries(int seriesId, CancellationToken cancellationToken);

        Task<IEnumerable<SeriesModel>> GetSeries(CancellationToken cancellationToken);

        Task<SeriesModel> GetSeriesById(int seriesId, CancellationToken cancellationToken);
    }
}