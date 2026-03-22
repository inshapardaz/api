using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class DeleteIssueRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber)
    : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; private set; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
}

public class DeleteIssueRequestHandler(IIssueRepository issueRepository, IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<DeleteIssueRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteIssueRequest> HandleAsync(DeleteIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        if (issue != null)
        {
            var contents = await issueRepository.GetIssueContents(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
            foreach (var content in contents)
            {
                await commandProcessor.SendAsync(new DeleteIssueContentRequest(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, content.Id), cancellationToken: cancellationToken);
            }

            await issueRepository.DeleteIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        }
        return await base.HandleAsync(command, cancellationToken);
    }
}
