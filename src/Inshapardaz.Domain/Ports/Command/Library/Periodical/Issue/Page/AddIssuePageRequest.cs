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

public class AddIssuePageRequest : LibraryBaseCommand
{
    public AddIssuePageRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int? accountId, IssuePageModel issue)
    : base(libraryId)
    {
        AccountId = accountId;
        IssuePage = issue;
        IssuePage.VolumeNumber = volumeNumber;
        IssuePage.PeriodicalId = periodicalId;
        IssuePage.IssueNumber = issueNumber;
    }

    public int? AccountId { get; }
    public IssuePageModel IssuePage { get; }

    public IssuePageModel Result { get; set; }

    public bool IsAdded { get; set; }
}

public class AddIssuePageRequestHandler : RequestHandlerAsync<AddIssuePageRequest>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IIssuePageRepository _issuePageRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public AddIssuePageRequestHandler(IIssueRepository issueRepository,
                                     IIssuePageRepository issuePageRepository,
                                     IAmACommandProcessor commandProcessor)
    {
        _issueRepository = issueRepository;
        _issuePageRepository = issuePageRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddIssuePageRequest> HandleAsync(AddIssuePageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var Issue = await _issueRepository.GetIssue(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, cancellationToken);
        if (Issue == null)
        {
            throw new BadRequestException();
        }

        var existingIssuePage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber, cancellationToken);

        var fileName = FilePathHelper.IssuePageContentFileName;
        var filePath = FilePathHelper.GetIssuePageContentPath(command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, fileName);
        var bookText = command.IssuePage.Text;

        var saveContentCommand = new SaveTextFileCommand(fileName, filePath, bookText)
        {
            MimeType = MimeTypes.Markdown,
            ExistingFileId = existingIssuePage?.FileId
        };

        await _commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);
        command.IssuePage.FileId = saveContentCommand.Result.Id;

        if (existingIssuePage == null)
        {
            command.Result = await _issuePageRepository.AddPage(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber, command.IssuePage.FileId, null, command.IssuePage.ArticleId, command.IssuePage.Status, cancellationToken);
            command.IsAdded = true;
        }
        else
        {
            command.Result = await _issuePageRepository.UpdatePage(command.LibraryId, command.IssuePage.PeriodicalId, command.IssuePage.VolumeNumber, command.IssuePage.IssueNumber, command.IssuePage.SequenceNumber, command.IssuePage.FileId, existingIssuePage.ImageId, command.IssuePage.ArticleId, command.IssuePage.Status, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
