using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Series;

public class GetSeriesQuery(int libraryId, int pageNumber, int pageSize)
    : LibraryBaseQuery<Page<SeriesModel>>(libraryId)
{
    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;

    public string Query { get; set; }
    public SeriesSortByType SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}

public class GetSeriesQueryHandler(ISeriesRepository seriesRepository, IFileRepository fileRepository)
    : QueryHandlerAsync<GetSeriesQuery, Page<SeriesModel>>
{
    [LibraryAuthorize(1)]
    public override async Task<Page<SeriesModel>> ExecuteAsync(GetSeriesQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var series = string.IsNullOrWhiteSpace(query.Query)
         ? await seriesRepository.GetSeries(query.LibraryId, query.PageNumber, query.PageSize, query.SortBy, query.SortDirection, cancellationToken)
         : await seriesRepository.FindSeries(query.LibraryId, query.Query, query.PageNumber, query.PageSize, query.SortBy, query.SortDirection, cancellationToken);

        foreach (var author in series.Data)
        {
            if (author != null && author.ImageId.HasValue)
            {
                author.ImageUrl = await ImageHelper.TryConvertToPublicFile(author.ImageId.Value, fileRepository, cancellationToken);
            }
        }

        return series;
    }
}
