using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetPeriodicalByIdQuery : IQuery<Periodical>
    {
        public GetPeriodicalByIdQuery(int periodicalId)
        {
            PeriodicalId = periodicalId;
        }

        public int PeriodicalId { get; }
    }

    public class GetPeriodicalByIdQueryHandler : QueryHandlerAsync<GetPeriodicalByIdQuery, Periodical>
    {
        private readonly IPeriodicalRepository _periodicalRepository;

        public GetPeriodicalByIdQueryHandler(IPeriodicalRepository periodicalRepository)
        {
            _periodicalRepository = periodicalRepository;
        }

        public override async Task<Periodical> ExecuteAsync(GetPeriodicalByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _periodicalRepository.GetPeriodicalById(command.PeriodicalId, cancellationToken);            
        }
    }
}

