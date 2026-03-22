using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

public class AssignIssuePageToUserRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int sequenceNumber,
    int? accountId)
    : LibraryBaseCommand(libraryId)
{
    public IssuePageModel Result { get; set; }
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public int SequenceNumber { get; set; } = sequenceNumber;
    public int? AccountId { get; private set; } = accountId;
}

public class AssignIssuePageToUserRequestHandler(IIssuePageRepository issuePageRepository)
    : RequestHandlerAsync<AssignIssuePageToUserRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AssignIssuePageToUserRequest> HandleAsync(AssignIssuePageToUserRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var page = await issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        if (page == null)
        {
            throw new BadRequestException();
        }

        if (page.Status == EditingStatus.Available || page.Status == EditingStatus.Typing)
        {
            command.Result = await issuePageRepository.UpdateWriterAssignment(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.AccountId, cancellationToken);
        }
        else if (page.Status == EditingStatus.Typed || page.Status == EditingStatus.InReview)
        {
            command.Result = await issuePageRepository.UpdateReviewerAssignment(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.AccountId, cancellationToken);
        }
        else
        {
            throw new BadRequestException("Page status does not allow it to be assigned");
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
