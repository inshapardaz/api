using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Series;

public class GetSeriesQuery : LibraryBaseQuery<Page<SeriesModel>>
{
    public GetSeriesQuery(int libraryId, int pageNumber, int pageSize)
        : base(libraryId)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageNumber { get; private set; }

    public int PageSize { get; private set; }

    public string Query { get; set; }
    public SeriesSortByType SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}

public class GetSeriesQueryHandler : QueryHandlerAsync<GetSeriesQuery, Page<SeriesModel>>
{
    private readonly ISeriesRepository _seriesRepository;
    private readonly IFileRepository _fileRepository;

    public GetSeriesQueryHandler(ISeriesRepository seriesRepository, IFileRepository fileRepository)
    {
        _seriesRepository = seriesRepository;
        _fileRepository = fileRepository;
    }

    [LibraryAuthorize(1)]
    public override async Task<Page<SeriesModel>> ExecuteAsync(GetSeriesQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var series = string.IsNullOrWhiteSpace(query.Query)
         ? await _seriesRepository.GetSeries(query.LibraryId, query.PageNumber, query.PageSize, query.SortBy, query.SortDirection, cancellationToken)
         : await _seriesRepository.FindSeries(query.LibraryId, query.Query, query.PageNumber, query.PageSize, query.SortBy, query.SortDirection, cancellationToken);

        foreach (var author in series.Data)
        {
            if (author != null && author.ImageId.HasValue)
            {
                author.ImageUrl = await ImageHelper.TryConvertToPublicFile(author.ImageId.Value, _fileRepository, cancellationToken);
            }
        }

        return series;
    }
}
