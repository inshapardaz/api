using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library;

public class UpdateLibraryRequest(int libraryId, LibraryModel library) : LibraryBaseCommand(libraryId)
{
    public LibraryModel Library { get; } = library;

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public LibraryModel Library { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateLibraryRequestHandler(ILibraryRepository libraryRepository)
    : RequestHandlerAsync<UpdateLibraryRequest>
{
    [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
    public override async Task<UpdateLibraryRequest> HandleAsync(UpdateLibraryRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);

        if (result == null)
        {
            var library = command.Library;
            library.Id = default;
            command.Result.Library = await libraryRepository.AddLibrary(library, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Library.Id = command.LibraryId;

            // These are never returned by the API (see LibraryRenderer), so a
            // client editing a library has no way to resubmit its current
            // value -- treat a blank submission as "leave unchanged" rather
            // than clobbering the stored secret with an empty string.
            if (string.IsNullOrWhiteSpace(command.Library.DatabaseConnection))
            {
                command.Library.DatabaseConnection = result.DatabaseConnection;
            }
            if (string.IsNullOrWhiteSpace(command.Library.FileStoreSource))
            {
                command.Library.FileStoreSource = result.FileStoreSource;
            }

            await libraryRepository.UpdateLibrary(command.Library, cancellationToken);
            command.Result.Library = command.Library;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
