using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library;

public class LibraryCheckerHandler<T>(ILibraryRepository libraryRepository) : RequestHandlerAsync<T>
    where T : LibraryBaseCommand
{
    public override async Task<T> HandleAsync(T command, CancellationToken cancellationToken)
    {
        var libraryCommand = command as LibraryBaseCommand;
        var library = await libraryRepository.GetLibraryById(libraryCommand.LibraryId, cancellationToken);

        if (library == null)
        {
            throw new NotFoundException();
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}

public class UseLibraryCheckAttribute(int step, HandlerTiming timing = HandlerTiming.Before)
    : RequestHandlerAttribute(step, timing)
{
    public override Type GetHandlerType() => typeof(LibraryCheckerHandler<>);
}
