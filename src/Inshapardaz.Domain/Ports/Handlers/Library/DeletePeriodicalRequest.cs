using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeletePeriodicalRequest : RequestBase
    {
        public DeletePeriodicalRequest(int periodicalId)
        {
            PeriodicalId = periodicalId;
        }

        public int PeriodicalId { get; }
    }

    public class DeletePeriodicalRequestHandler : RequestHandlerAsync<DeletePeriodicalRequest>
    {
        private readonly IPeriodicalRepository _periodicalRepository;

        public DeletePeriodicalRequestHandler(IPeriodicalRepository periodicalRepository)
        {
            _periodicalRepository = periodicalRepository;
        }

        public override async Task<DeletePeriodicalRequest> HandleAsync(DeletePeriodicalRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _periodicalRepository.DeletePeriodical(command.PeriodicalId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}