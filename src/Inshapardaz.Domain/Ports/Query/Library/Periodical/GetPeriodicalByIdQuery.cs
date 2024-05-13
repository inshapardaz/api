using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Query.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical;

public class GetPeriodicalByIdQuery : LibraryBaseQuery<PeriodicalModel>
{
    public GetPeriodicalByIdQuery(int libraryId, int periodicalId)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
    }

    public int PeriodicalId { get; }
}

public class GetPeriodicalByIdQueryHandler : QueryHandlerAsync<GetPeriodicalByIdQuery, PeriodicalModel>
{
    private readonly IPeriodicalRepository _periodicalRepository;

    public GetPeriodicalByIdQueryHandler(IPeriodicalRepository periodicalRepository)
    {
        _periodicalRepository = periodicalRepository;
    }

    public override async Task<PeriodicalModel> ExecuteAsync(GetPeriodicalByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        return await _periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);
    }
}
