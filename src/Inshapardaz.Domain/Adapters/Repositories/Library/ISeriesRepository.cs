using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface ISeriesRepository
    {
        Task<SeriesModel> AddSeries(int libraryId, SeriesModel series, CancellationToken cancellationToken);

        Task UpdateSeries(int libraryId, SeriesModel series, CancellationToken cancellationToken);

        Task DeleteSeries(int libraryId, int seriesId, CancellationToken cancellationToken);

        Task<IEnumerable<SeriesModel>> GetSeries(int libraryId, CancellationToken cancellationToken);

        Task<SeriesModel> GetSeriesById(int libraryId, int seriesId, CancellationToken cancellationToken);
    }
}
