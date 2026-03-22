using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library;

public class DeleteLibraryRequest(int libraryId) : LibraryBaseCommand(libraryId);

public class DeleteLibraryRequestHandler(
    ILibraryRepository libraryRepository,
    IFileRepository fileRepository,
    IFileStorage fileStore)
    : RequestHandlerAsync<DeleteLibraryRequest>
{
    private readonly IFileRepository _fileRepository = fileRepository;
    private readonly IFileStorage _fileStore = fileStore;

    [LibraryAuthorize(1, Role.Admin)]
    public override async Task<DeleteLibraryRequest> HandleAsync(DeleteLibraryRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var library = await libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
        if (library != null)
        {
            await libraryRepository.DeleteLibrary(command.LibraryId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
