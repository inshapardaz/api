using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;

public class AddIssueArticleContentRequest : LibraryBaseCommand
{
    public AddIssueArticleContentRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string contents, string language)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        SequenceNumber = sequenceNumber;
        Content = contents;
        Language = language;
    }

    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public int SequenceNumber { get; }
    public string Content { get; }

    public string Language { get; set; }

    public IssueArticleContentModel Result { get; set; }
}

public class AddArticleContentRequestHandler : RequestHandlerAsync<AddIssueArticleContentRequest>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IIssueArticleRepository _articleRepository;
    private readonly ILibraryRepository _libraryRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public AddArticleContentRequestHandler(IIssueRepository issueRepository, 
        IIssueArticleRepository chapterRepository, 
        ILibraryRepository libraryRepository, 
        IAmACommandProcessor commandProcessor)
    {
        _issueRepository = issueRepository;
        _articleRepository = chapterRepository;
        _libraryRepository = libraryRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddIssueArticleContentRequest> HandleAsync(AddIssueArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (string.IsNullOrWhiteSpace(command.Language))
        {
            var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Language = library.Language;
        }

        var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        if (issue == null)
        {
            throw new BadRequestException();
        }

        var article = await _articleRepository.GetArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

        if (article != null)
        {
            var fileName = FilePathHelper.GetIssueArticleContentFileName(command.Language);
            var saveFileCommand = new SaveTextFileCommand(
                fileName,
                FilePathHelper.GetIssueArticleContentPath(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, article.Id, fileName),
                command.Content)
            {
                MimeType = MimeTypes.Markdown,
            };
            await _commandProcessor.SendAsync(saveFileCommand, cancellationToken: cancellationToken);

            command.Result = await _articleRepository.AddArticleContent(
                command.LibraryId,
                new IssueArticleContentModel
                {
                    PeriodicalId = command.PeriodicalId,
                    VolumeNumber = command.VolumeNumber,
                    IssueNumber = command.IssueNumber,
                    SequenceNumber = command.SequenceNumber,
                    Language = command.Language,
                    FileId = saveFileCommand.Result.Id,
                },
                cancellationToken);

            command.Result.Text = command.Content;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
