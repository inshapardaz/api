using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class DeleteIssueRatingRequest(int libraryId, int accountId, int periodicalId, int volumeNumber, int issueNumber)
    : LibraryBaseCommand(libraryId)
{
    public int AccountId { get; } = accountId;
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
}

public class DeleteIssueRatingRequestHandler(IIssueRepository issueRepository)
    : RequestHandlerAsync<DeleteIssueRatingRequest>
{
    [LibraryAuthorize(1, Role.Reader, Role.Writer, Role.Admin, Role.LibraryAdmin)]
    public override async Task<DeleteIssueRatingRequest> HandleAsync(DeleteIssueRatingRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

        if (issue != null)
        {
            await issueRepository.DeleteIssueRating(command.LibraryId, command.AccountId, issue.Id, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
