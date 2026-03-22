using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

public class DeleteIssuePageRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int sequenceNumber)
    : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;

    public int SequenceNumber { get; } = sequenceNumber;
}

public class DeleteIssuePageRequestHandler(
    IIssuePageRepository issuePageRepository,
    IFileRepository fileRepository,
    IFileStorage fileStorage)
    : RequestHandlerAsync<DeleteIssuePageRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteIssuePageRequest> HandleAsync(DeleteIssuePageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issuePage = await issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

        if (issuePage != null)
        {
            if (issuePage.ImageId.HasValue)
            {
                var existingImage = await fileRepository.GetFileById(issuePage.ImageId.Value, cancellationToken);
                if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                {
                    await fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
                }

                await fileRepository.DeleteFile(existingImage.Id, cancellationToken);
                await issuePageRepository.DeletePageImage(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
            }

            await issuePageRepository.DeletePage(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
