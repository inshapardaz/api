﻿using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical;

public class DeletePeriodicalRequest : LibraryBaseCommand
{
    public DeletePeriodicalRequest(int libraryId, int periodicalId)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
    }

    public int PeriodicalId { get; }
}

public class DeletePeriodicalRequestHandler : RequestHandlerAsync<DeletePeriodicalRequest>
{
    private readonly IPeriodicalRepository _periodicalRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public DeletePeriodicalRequestHandler(IPeriodicalRepository periodicalRepository, IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _periodicalRepository = periodicalRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeletePeriodicalRequest> HandleAsync(DeletePeriodicalRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var periodical = await _periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);
        if (periodical != null)
        {
            await _periodicalRepository.DeletePeriodical(command.LibraryId, command.PeriodicalId, cancellationToken);

            if (!string.IsNullOrWhiteSpace(periodical.ImageUrl))
            {
                await _fileStorage.TryDeleteImage(periodical.ImageUrl, cancellationToken);
                await _fileRepository.DeleteFile(periodical.ImageId.Value, cancellationToken);
            }
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
