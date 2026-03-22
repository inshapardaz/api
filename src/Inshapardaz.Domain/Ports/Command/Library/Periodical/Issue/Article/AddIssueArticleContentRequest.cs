using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;

public class AddIssueArticleContentRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int sequenceNumber,
    string contents,
    string language)
    : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public int SequenceNumber { get; } = sequenceNumber;
    public string Content { get; } = contents;

    public string Language { get; set; } = language;

    public IssueArticleContentModel Result { get; set; }
}

public class AddArticleContentRequestHandler(
    IIssueRepository issueRepository,
    IIssueArticleRepository chapterRepository,
    ILibraryRepository libraryRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<AddIssueArticleContentRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddIssueArticleContentRequest> HandleAsync(AddIssueArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (string.IsNullOrWhiteSpace(command.Language))
        {
            var library = await libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Language = library.Language;
        }

        var issue = await issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        if (issue == null)
        {
            throw new BadRequestException();
        }

        var article = await chapterRepository.GetIssueArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

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
            await commandProcessor.SendAsync(saveFileCommand, cancellationToken: cancellationToken);

            command.Result = await chapterRepository.AddIssueArticleContent(
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
