using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

public class UpdateIssuePageSequenceRequest : LibraryBaseCommand
{
    public UpdateIssuePageSequenceRequest(int libraryId,
        int periodicalId,
        int volumeNumber,
        int issueNumber,
        int oldSequenceNumber,
        int newSequenceNumber)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        OldSequenceNumber = oldSequenceNumber;
        NewSequenceNumber = newSequenceNumber;
    }

    public IEnumerable<IssuePageModel> BookPages { get; }
    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public int OldSequenceNumber { get; }
    public int NewSequenceNumber { get; }
}

public class UpdateIssuePageSequenceRequestHandler : RequestHandlerAsync<UpdateIssuePageSequenceRequest>
{
    private readonly IIssuePageRepository _issuePageRepository;

    public UpdateIssuePageSequenceRequestHandler(IIssuePageRepository issuePageRepository)
    {
        _issuePageRepository = issuePageRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssuePageSequenceRequest> HandleAsync(UpdateIssuePageSequenceRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        // No Change in page sequence
        if (command.OldSequenceNumber == command.NewSequenceNumber)
        {
            return await base.HandleAsync(command, cancellationToken);
        }

        var page = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.OldSequenceNumber, cancellationToken);

        // Check if the page exist
        if (page == null)
        {
            throw new NotFoundException();
        }

        await _issuePageRepository.UpdatePageSequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.OldSequenceNumber, command.NewSequenceNumber, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
