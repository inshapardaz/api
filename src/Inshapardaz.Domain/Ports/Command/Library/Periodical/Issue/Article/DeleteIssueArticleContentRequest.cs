using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;

public class DeleteIssueArticleContentRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int articleId,
    string language)
    : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public int SequenceNumber { get; } = articleId;
    public string Language { get; } = language;
}

public class DeleteArticleContentRequestHandler(
    IIssueArticleRepository articleRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<DeleteIssueArticleContentRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteIssueArticleContentRequest> HandleAsync(DeleteIssueArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var content = await articleRepository.GetIssueArticleContent(command.LibraryId, new IssueArticleContentModel
        {
            PeriodicalId = command.PeriodicalId,
            VolumeNumber = command.VolumeNumber,
            IssueNumber = command.IssueNumber,
            SequenceNumber = command.SequenceNumber,
            Language = command.Language
        }, cancellationToken);

        if (content != null)
        {
            await commandProcessor.SendAsync(new DeleteTextFileCommand(content.FileId), cancellationToken: cancellationToken);
            await articleRepository.DeleteIssueArticleContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
