using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Periodical
{
    public class AddPeriodicalRequest : LibraryBaseCommand
    {
        public AddPeriodicalRequest(int libraryId, PeriodicalModel periodical)
            : base(libraryId)
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
