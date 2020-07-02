using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddPeriodicalRequest : LibraryAuthorisedCommand
    {
        public AddPeriodicalRequest(ClaimsPrincipal claims, int libraryId, PeriodicalModel periodical)
            : base(claims, libraryId)
        {
            Periodical = periodical;
        }

        public PeriodicalModel Periodical { get; }

        public PeriodicalModel Result { get; set; }
    }

    public class AddPeriodicalRequestHandler : RequestHandlerAsync<AddPeriodicalRequest>
    {
        private readonly IPeriodicalRepository _periodicalRepository;

        public AddPeriodicalRequestHandler(IPeriodicalRepository periodicalRepository)
        {
            _periodicalRepository = periodicalRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<AddPeriodicalRequest> HandleAsync(AddPeriodicalRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _periodicalRepository.AddPeriodical(command.LibraryId, command.Periodical, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
