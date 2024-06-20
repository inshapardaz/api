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
    public UpdateIssuePageRequest(int libraryId,
                            int periodicalId,
                            int volumeNumber,
                            int issueNumber,
                            int sequenceNumber,
                            string text,
                            long? articleId,
                            int? writeAccountId,
                            int? reviewerAccountId)
    : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        SequenceNumber = sequenceNumber;
        Text = text;
        ArticleId = articleId;
        WriterAccountId = writeAccountId;
        ReviewerAccountId = reviewerAccountId;
    }

    public UpdateIssuePageRequest(int libraryId, IssuePageModel model) 
        : this(libraryId, model.PeriodicalId, model.VolumeNumber, model.IssueNumber, model.SequenceNumber, model.Text, model.ArticleId, model.WriterAccountId, model.ReviewerAccountId)
    {
    }

    public RequestResult Result { get; set; } = new RequestResult();
    public int PeriodicalId { get; private set; }
    public int VolumeNumber { get; private set; }
    public int IssueNumber { get; private set; }
    public int SequenceNumber { get; set; }
    public string Text { get; set; }
    public long? ArticleId { get; set; }
    public int? WriterAccountId { get; set; }
    public int? ReviewerAccountId { get; set; }
    public EditingStatus Status { get; set; }

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
        var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        if (issue == null)
        {
            throw new BadRequestException();
        }

        var existingIssuePage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

        var fileName = FilePathHelper.IssuePageContentFileName;
        var filePath = FilePathHelper.GetIssuePageContentPath(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, fileName);

        var saveContentCommand = new SaveTextFileCommand(fileName, filePath, command.Text)
        {
            MimeType = MimeTypes.Markdown,
            ExistingFileId = existingIssuePage?.FileId
        };

        await _commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);

        if (existingIssuePage == null)
        {
            command.Result.IssuePage = await _issuePageRepository.AddPage(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, saveContentCommand.Result.Id, 0, command.ArticleId, command.Status, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Result.IssuePage = await _issuePageRepository.UpdatePage(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, saveContentCommand.Result.Id, existingIssuePage.ImageId ?? 0, command.ArticleId, command.Status, cancellationToken);
        }

        command.Result.IssuePage.FileId = saveContentCommand.Result.Id;
        command.Result.IssuePage.Text = command.Text;

        var previousPage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber - 1, cancellationToken);
        var nextPage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber + 1, cancellationToken);

        command.Result.IssuePage.PreviousPage = previousPage;
        command.Result.IssuePage.NextPage = nextPage;

        return await base.HandleAsync(command, cancellationToken);
    }
}
