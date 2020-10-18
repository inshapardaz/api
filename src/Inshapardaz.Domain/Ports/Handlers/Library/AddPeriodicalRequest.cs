using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
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
        private readonly ILibraryRepository _libraryRepository;
        private readonly IPeriodicalRepository _periodicalRepository;

        public AddPeriodicalRequestHandler(ILibraryRepository libraryRepository, IPeriodicalRepository periodicalRepository)
        {
            _libraryRepository = libraryRepository;
            _periodicalRepository = periodicalRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before, Permission.Admin, Permission.LibraryAdmin, Permission.Writer)]
        public override async Task<AddPeriodicalRequest> HandleAsync(AddPeriodicalRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);

            if (library == null || !library.SupportsPeriodicals)
            {
                throw new BadRequestException();
            }

            command.Result = await _periodicalRepository.AddPeriodical(command.LibraryId, command.Periodical, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
