using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

public class UpdateIssuePageRequest : LibraryBaseCommand
{
    public UpdateIssuePageRequest(int libraryId, IssuePageModel model) 
        : base(libraryId)
    {
        IssuePage = model;
    }
    public IssuePageModel IssuePage { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public IssuePageModel IssuePage { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateIssuePageRequestHandler : RequestHandlerAsync<UpdateIssuePageRequest>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IIssuePageRepository _issuePageRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public UpdateIssuePageRequestHandler(IIssueRepository issueRepository,
                                     IIssuePageRepository issuePageRepository,
                                     IAmACommandProcessor commandProcessor)
    {
        _issueRepository = issueRepository;
        _issuePageRepository = issuePageRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssuePageRequest> HandleAsync(UpdateIssuePageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await _issueRepository.GetIssue(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, cancellationToken);
        if (issue == null)
        {
            throw new BadRequestException();
        }

        var existingIssuePage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber, cancellationToken);

        var fileName = FilePathHelper.IssuePageContentFileName;
        var filePath = FilePathHelper.GetIssuePageContentPath(command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, fileName);

        var saveContentCommand = new SaveTextFileCommand(fileName, filePath, command.IssuePage.Text)
        {
            MimeType = MimeTypes.Markdown,
            ExistingFileId = existingIssuePage?.FileId
        };

        await _commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);
        command.IssuePage.FileId = saveContentCommand.Result.Id;
        command.IssuePage.ImageId = existingIssuePage?.ImageId;
        
        if (existingIssuePage == null)
        {
            command.Result.IssuePage = await _issuePageRepository.AddPage(command.LibraryId, command.IssuePage, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Result.IssuePage = await _issuePageRepository.UpdatePage(command.LibraryId, command.IssuePage, cancellationToken);
        }

        command.Result.IssuePage.FileId = saveContentCommand.Result.Id;
        command.Result.IssuePage.Text = command.IssuePage.Text;

        var previousPage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber - 1, cancellationToken);
        var nextPage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber + 1, cancellationToken);

        command.Result.IssuePage.PreviousPage = previousPage;
        command.Result.IssuePage.NextPage = nextPage;

        return await base.HandleAsync(command, cancellationToken);
    }
}
