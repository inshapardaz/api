using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class DeleteIssueRequest : LibraryBaseCommand
{
    public DeleteIssueRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
    }

    public int PeriodicalId { get; private set; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
}

public class DeleteIssueRequestHandler : RequestHandlerAsync<DeleteIssueRequest>
{
    private readonly IIssueRepository _issueRepository;

    public DeleteIssueRequestHandler(IIssueRepository issueRepository)
    {
        _issueRepository = issueRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteIssueRequest> HandleAsync(DeleteIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await _issueRepository.DeleteIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
