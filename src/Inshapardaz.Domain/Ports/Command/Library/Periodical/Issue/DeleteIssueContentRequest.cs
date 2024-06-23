using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class DeleteIssueContentRequest : LibraryBaseCommand
{
    public DeleteIssueContentRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, long contentId)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        ContentId = contentId;
    }

    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public long ContentId { get; }
}

public class DeleteIssueContentRequestHandler : RequestHandlerAsync<DeleteIssueContentRequest>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public DeleteIssueContentRequestHandler(IIssueRepository issueRepository, IAmACommandProcessor commandProcessor)
    {
        _issueRepository = issueRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteIssueContentRequest> HandleAsync(DeleteIssueContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var content = await _issueRepository.GetIssueContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ContentId, cancellationToken);
        if (content != null)
        {
            await _commandProcessor.SendAsync(new DeleteFileCommand(content.FileId), cancellationToken: cancellationToken);
            await _issueRepository.DeleteIssueContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ContentId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
