using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical;

public class DeletePeriodicalRequest(int libraryId, int periodicalId) : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
}

public class DeletePeriodicalRequestHandler(
    IPeriodicalRepository periodicalRepository,
    IFileRepository fileRepository,
    IFileStorage fileStorage)
    : RequestHandlerAsync<DeletePeriodicalRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeletePeriodicalRequest> HandleAsync(DeletePeriodicalRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var periodical = await periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);
        if (periodical != null)
        {
            await periodicalRepository.DeletePeriodical(command.LibraryId, command.PeriodicalId, cancellationToken);

            if (!string.IsNullOrWhiteSpace(periodical.ImageUrl))
            {
                await fileStorage.TryDeleteImage(periodical.ImageUrl, cancellationToken);
                await fileRepository.DeleteFile(periodical.ImageId.Value, cancellationToken);
            }
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
