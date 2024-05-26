using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library;

public interface ISeriesRepository
{
    Task<SeriesModel> AddSeries(int libraryId, SeriesModel series, CancellationToken cancellationToken);

    Task UpdateSeries(int libraryId, SeriesModel series, CancellationToken cancellationToken);

    Task DeleteSeries(int libraryId, int seriesId, CancellationToken cancellationToken);

    Task<Page<SeriesModel>> GetSeries(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<Page<SeriesModel>> FindSeries(int libraryId, string query, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<SeriesModel> GetSeriesById(int libraryId, int seriesId, CancellationToken cancellationToken);

    Task UpdateSeriesImage(int libraryId, int seriesId, long imageId, CancellationToken cancellationToken);
}
