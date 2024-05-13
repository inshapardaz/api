using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

public class AssignIssuePageRequest : LibraryBaseCommand
{
    public AssignIssuePageRequest(int libraryId,
                            int periodicalId,
                            int volumeNumber,
                            int issueNumber,
                            int sequenceNumber,
                            int? accountId)
    : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        SequenceNumber = sequenceNumber;
        AccountId = accountId;
    }

    public IssuePageModel Result { get; set; }
    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public int SequenceNumber { get; set; }
    public int? AccountId { get; private set; }
}

public class AssignIssuePageRequestHandler : RequestHandlerAsync<AssignIssuePageRequest>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IIssuePageRepository _issuePageRepository;

    public AssignIssuePageRequestHandler(IIssueRepository issueRepository,
                                     IIssuePageRepository issuePageRepository)
    {
        _issueRepository = issueRepository;
        _issuePageRepository = issuePageRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AssignIssuePageRequest> HandleAsync(AssignIssuePageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var page = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        if (page == null)
        {
            throw new BadRequestException();
        }

        if (page.Status == EditingStatus.Available || page.Status == EditingStatus.Typing)
        {
            command.Result = await _issuePageRepository.UpdateWriterAssignment(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.AccountId, cancellationToken);
        }
        else if (page.Status == EditingStatus.Typed || page.Status == EditingStatus.InReview)
        {
            command.Result = await _issuePageRepository.UpdateReviewerAssignment(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.AccountId, cancellationToken);
        }
        else
        {
            throw new BadRequestException("Page status does not allow it to be assigned");
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
