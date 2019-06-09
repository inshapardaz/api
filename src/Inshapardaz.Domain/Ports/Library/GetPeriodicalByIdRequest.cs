using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetPeriodicalByIdRequest : RequestBase
    {
        public GetPeriodicalByIdRequest(int periodicalId)
        {
            PeriodicalId = periodicalId;
        }

        public Periodical Result { get; set; }
        public int PeriodicalId { get; }
    }

    public class GetPeriodicalByIdRequestHandler : RequestHandlerAsync<GetPeriodicalByIdRequest>
    {
        private readonly IPeriodicalRepository _periodicalRepository;

        public GetPeriodicalByIdRequestHandler(IPeriodicalRepository periodicalRepository)
        {
            _periodicalRepository = periodicalRepository;
        }

        public override async Task<GetPeriodicalByIdRequest> HandleAsync(GetPeriodicalByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _periodicalRepository.GetPeriodicalById(command.PeriodicalId, cancellationToken);            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

