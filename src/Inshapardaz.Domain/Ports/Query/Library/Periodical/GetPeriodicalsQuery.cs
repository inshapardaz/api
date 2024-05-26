using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical;

public class GetPeriodicalsQuery : LibraryBaseQuery<Page<PeriodicalModel>>
{
    public GetPeriodicalsQuery(int libraryId, int pageNumber, int pageSize)
        : base(libraryId)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageNumber { get; private set; }

    public int PageSize { get; private set; }

    public string Query { get; set; }
    public PeriodicalFilter Filter { get; set; }
    public PeriodicalSortByType SortBy { get; set; }
    public SortDirection Direction { get; set; }
}

public class GetPeriodicalsQueryHandler : QueryHandlerAsync<GetPeriodicalsQuery, Page<PeriodicalModel>>
{
    private readonly IPeriodicalRepository _periodicalRepository;

    public GetPeriodicalsQueryHandler(IPeriodicalRepository periodicalRepository)
    {
        _periodicalRepository = periodicalRepository;
    }

    public override async Task<Page<PeriodicalModel>> ExecuteAsync(GetPeriodicalsQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        return await _periodicalRepository.GetPeriodicals(query.LibraryId, query.Query, query.PageNumber, query.PageSize, query.Filter, query.SortBy, query.Direction, cancellationToken);
    }
}
