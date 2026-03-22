using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class DeleteIssueContentRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    long contentId)
    : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public long ContentId { get; } = contentId;
}

public class DeleteIssueContentRequestHandler(IIssueRepository issueRepository, IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<DeleteIssueContentRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteIssueContentRequest> HandleAsync(DeleteIssueContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var content = await issueRepository.GetIssueContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ContentId, cancellationToken);
        if (content != null)
        {
            await commandProcessor.SendAsync(new DeleteFileCommand(content.FileId), cancellationToken: cancellationToken);
            await issueRepository.DeleteIssueContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ContentId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
