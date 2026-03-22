using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

public class UpdateIssuePageSequenceRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int oldSequenceNumber,
    int newSequenceNumber)
    : LibraryBaseCommand(libraryId)
{
    public IEnumerable<IssuePageModel> BookPages { get; }
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public int OldSequenceNumber { get; } = oldSequenceNumber;
    public int NewSequenceNumber { get; } = newSequenceNumber;
}

public class UpdateIssuePageSequenceRequestHandler(IIssuePageRepository issuePageRepository)
    : RequestHandlerAsync<UpdateIssuePageSequenceRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssuePageSequenceRequest> HandleAsync(UpdateIssuePageSequenceRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        // No Change in page sequence
        if (command.OldSequenceNumber == command.NewSequenceNumber)
        {
            return await base.HandleAsync(command, cancellationToken);
        }

        var page = await issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.OldSequenceNumber, cancellationToken);

        // Check if the page exist
        if (page == null)
        {
            throw new NotFoundException();
        }

        await issuePageRepository.UpdatePageSequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.OldSequenceNumber, command.NewSequenceNumber, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
