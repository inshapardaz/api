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

public class UpdateIssueArticleContentRequest : LibraryBaseCommand
{
    public UpdateIssueArticleContentRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string content, string language)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        SequenceNumber = sequenceNumber;
        Content = content;
        Language = language;
    }

    public string Language { get; set; }
    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public int SequenceNumber { get; }
    public string Content { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public IssueArticleContentModel Content { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateArticleContentRequestHandler : RequestHandlerAsync<UpdateIssueArticleContentRequest>
{
    private readonly IIssueArticleRepository _articleRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly ILibraryRepository _libraryRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public UpdateArticleContentRequestHandler(IIssueArticleRepository articleRepository,
        IIssueRepository issueRepository, 
        ILibraryRepository libraryRepository, 
        IAmACommandProcessor commandProcessor)
    {
        _articleRepository = articleRepository;
        _issueRepository = issueRepository;
        _libraryRepository = libraryRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssueArticleContentRequest> HandleAsync(UpdateIssueArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var article = await _articleRepository.GetIssueArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

        if (article == null)
        {
            throw new BadRequestException();
        }

        if (string.IsNullOrWhiteSpace(command.Language))
        {
            var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Language = library.Language;
        }

        var content = await _articleRepository.GetIssueArticleContentById(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.Language, cancellationToken);

        var fileName = FilePathHelper.GetIssueArticleContentFileName(command.Language);
        var saveFileCommand = new SaveTextFileCommand(
            fileName,
            FilePathHelper.GetIssueArticleContentPath(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, article.Id, fileName),
            command.Content)
        {
            MimeType = MimeTypes.Markdown,
            ExistingFileId = content?.FileId
        };
        await _commandProcessor.SendAsync(saveFileCommand, cancellationToken: cancellationToken);


        if (content == null)
        {
            command.Result.Content = await _articleRepository.AddIssueArticleContent(
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
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Result.Content = await _articleRepository.UpdateIssueArticleContent(command.LibraryId,
                                                    new IssueArticleContentModel
                                                    {
                                                        Id = content.Id,
                                                        PeriodicalId = command.PeriodicalId,
                                                        VolumeNumber = command.VolumeNumber,
                                                        IssueNumber = command.IssueNumber,
                                                        SequenceNumber = command.SequenceNumber,
                                                        Language = command.Language,
                                                        FileId = content.FileId,
                                                    },
                                                    cancellationToken);
        }

        if (command.Result.Content != null)
        {
            command.Result.Content.Text = command.Content;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
