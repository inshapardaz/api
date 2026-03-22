using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

public class DeleteIssuePageImageRequest(
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

public class DeleteIssuePageImageRequestHandler(
    IIssuePageRepository issuePageRepository,
    IFileRepository fileRepository,
    IFileStorage fileStorage)
    : RequestHandlerAsync<DeleteIssuePageImageRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteIssuePageImageRequest> HandleAsync(DeleteIssuePageImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issuePage = await issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

        if (issuePage != null && issuePage.ImageId.HasValue)
        {
            var existingImage = await fileRepository.GetFileById(issuePage.ImageId.Value, cancellationToken);
            if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
            {
                await fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
            }

            await fileRepository.DeleteFile(existingImage.Id, cancellationToken);
            await issuePageRepository.DeletePageImage(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
