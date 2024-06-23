using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
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
    private readonly IAmACommandProcessor _commandProcessor;

    public DeleteIssueRequestHandler(IIssueRepository issueRepository, IAmACommandProcessor commandProcessor)
    {
        _issueRepository = issueRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteIssueRequest> HandleAsync(DeleteIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        if (issue != null)
        {
            var contents = await _issueRepository.GetIssueContents(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
            foreach (var content in contents)
            {
                await _commandProcessor.SendAsync(new DeleteIssueContentRequest(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, content.Id), cancellationToken: cancellationToken);
            }

            await _issueRepository.DeleteIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        }
        return await base.HandleAsync(command, cancellationToken);
    }
}
