using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical;

public class GetPeriodicalsQuery(int libraryId, int pageNumber, int pageSize)
    : LibraryBaseQuery<Page<PeriodicalModel>>(libraryId)
{
    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;

    public string Query { get; set; }
    public PeriodicalFilter Filter { get; set; }
    public PeriodicalSortByType SortBy { get; set; }
    public SortDirection Direction { get; set; }
}

public class GetPeriodicalsQueryHandler(IPeriodicalRepository periodicalRepository)
    : QueryHandlerAsync<GetPeriodicalsQuery, Page<PeriodicalModel>>
{
    public override async Task<Page<PeriodicalModel>> ExecuteAsync(GetPeriodicalsQuery query, CancellationToken cancellationToken = new CancellationToken()) => await periodicalRepository.GetPeriodicals(query.LibraryId, query.Query, query.PageNumber, query.PageSize, query.Filter, query.SortBy, query.Direction, cancellationToken);
}
