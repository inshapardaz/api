using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddPeriodicalRequest : RequestBase
    {
        public AddPeriodicalRequest(Periodical periodical)
        {
            Periodical = periodical;
        }

        public Periodical Periodical { get; }

        public Periodical Result { get; set; }
    }

    public class AddPeriodicalRequestHandler : RequestHandlerAsync<AddPeriodicalRequest>
    {
        private readonly IPeriodicalRepository _periodicalRepository;

        public AddPeriodicalRequestHandler(IPeriodicalRepository periodicalRepository)
        {
            _periodicalRepository = periodicalRepository;
        }

        public override async Task<AddPeriodicalRequest> HandleAsync(AddPeriodicalRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result= await _periodicalRepository.AddPeriodical(command.Periodical, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}
