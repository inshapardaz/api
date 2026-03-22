using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Series;

public class GetSeriesByIdQuery(int libraryId, int seriesId) : LibraryBaseQuery<SeriesModel>(libraryId)
{
    public int SeriesId { get; } = seriesId;
}

public class GetSeriesByIdQueryHandler(
    ISeriesRepository seriesRepository,
    IBookRepository bookRepository,
    IFileRepository fileRepository)
    : QueryHandlerAsync<GetSeriesByIdQuery, SeriesModel>
{
    private readonly IBookRepository _bookRepository = bookRepository;

    [LibraryAuthorize(1)]
    public override async Task<SeriesModel> ExecuteAsync(GetSeriesByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var series = await seriesRepository.GetSeriesById(command.LibraryId, command.SeriesId, cancellationToken);

        if (series != null && series.ImageId.HasValue)
        {
            series.ImageUrl = await ImageHelper.TryConvertToPublicFile(series.ImageId.Value, fileRepository, cancellationToken);
        }

        return series;
    }
}
