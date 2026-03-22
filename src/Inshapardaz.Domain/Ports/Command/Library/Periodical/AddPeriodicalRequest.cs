using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical;

public class AddPeriodicalRequest(int libraryId, PeriodicalModel periodical) : LibraryBaseCommand(libraryId)
{
    public PeriodicalModel Periodical { get; } = periodical;

    public PeriodicalModel Result { get; set; }
}

public class AddPeriodicalRequestHandler(
    ILibraryRepository libraryRepository,
    IPeriodicalRepository periodicalRepository)
    : RequestHandlerAsync<AddPeriodicalRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddPeriodicalRequest> HandleAsync(AddPeriodicalRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var library = await libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);

        if (library == null || !library.SupportsPeriodicals)
        {
            throw new BadRequestException();
        }

        command.Result = await periodicalRepository.AddPeriodical(command.LibraryId, command.Periodical, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
