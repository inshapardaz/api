using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical;

public class GetPeriodicalByIdQuery(int libraryId, int periodicalId) : LibraryBaseQuery<PeriodicalModel>(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
}

public class GetPeriodicalByIdQueryHandler(IPeriodicalRepository periodicalRepository)
    : QueryHandlerAsync<GetPeriodicalByIdQuery, PeriodicalModel>
{
    public override async Task<PeriodicalModel> ExecuteAsync(GetPeriodicalByIdQuery command, CancellationToken cancellationToken = new CancellationToken()) => await periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);
}
