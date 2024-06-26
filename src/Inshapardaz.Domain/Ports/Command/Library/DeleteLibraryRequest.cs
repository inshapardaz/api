﻿using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library;

public class DeleteLibraryRequest : LibraryBaseCommand
{
    public DeleteLibraryRequest(int libraryId)
        : base(libraryId)
    {
    }
}

public class DeleteLibraryRequestHandler : RequestHandlerAsync<DeleteLibraryRequest>
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStore;

    public DeleteLibraryRequestHandler(ILibraryRepository libraryRepository, IFileRepository fileRepository, IFileStorage fileStore)
    {
        _libraryRepository = libraryRepository;
        _fileRepository = fileRepository;
        _fileStore = fileStore;
    }

    [LibraryAuthorize(1, Role.Admin)]
    public override async Task<DeleteLibraryRequest> HandleAsync(DeleteLibraryRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
        if (library != null)
        {
            await _libraryRepository.DeleteLibrary(command.LibraryId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
