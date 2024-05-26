using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library;

public class LibraryCheckerHandler<T> : RequestHandlerAsync<T> where T : LibraryBaseCommand
{
    private readonly ILibraryRepository _libraryRepository;

    public LibraryCheckerHandler(ILibraryRepository libraryRepository)
    {
        _libraryRepository = libraryRepository;
    }

    public override async Task<T> HandleAsync(T command, CancellationToken cancellationToken)
    {
        var libraryCommand = command as LibraryBaseCommand;
        var library = await _libraryRepository.GetLibraryById(libraryCommand.LibraryId, cancellationToken);

        if (library == null)
        {
            throw new NotFoundException();
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}

public class UseLibraryCheckAttribute : RequestHandlerAttribute
{

    public UseLibraryCheckAttribute(int step, HandlerTiming timing = HandlerTiming.Before)
        : base(step, timing)
    { }


    public override Type GetHandlerType()
    {
        return typeof(LibraryCheckerHandler<>);
    }
}
