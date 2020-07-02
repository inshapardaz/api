using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdatePeriodicalRequest : LibraryAuthorisedCommand
    {
        public UpdatePeriodicalRequest(ClaimsPrincipal claims, int libraryId, PeriodicalModel periodical)
            : base(claims, libraryId)
        {
            Periodical = periodical;
        }

        public PeriodicalModel Periodical { get; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public PeriodicalModel Periodical { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdatePeriodicalRequestHandler : RequestHandlerAsync<UpdatePeriodicalRequest>
    {
        private readonly IPeriodicalRepository _periodicalRepository;

        public UpdatePeriodicalRequestHandler(IPeriodicalRepository periodicalRepository)
        {
            _periodicalRepository = periodicalRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<UpdatePeriodicalRequest> HandleAsync(UpdatePeriodicalRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _periodicalRepository.GetPeriodicalById(command.LibraryId, command.Periodical.Id, cancellationToken);

            if (result == null)
            {
                var periodical = command.Periodical;
                periodical.Id = default(int);
                command.Result.Periodical = await _periodicalRepository.AddPeriodical(command.LibraryId, periodical, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _periodicalRepository.UpdatePeriodical(command.LibraryId, command.Periodical, cancellationToken);
                command.Result.Periodical = command.Periodical;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
