using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;

public class DeleteIssueArticleRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int sequenceNumber)
    : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public int SequenceNumber { get; } = sequenceNumber;
}

public class DeleteIssueArticleRequestHandler(
    IIssueArticleRepository articleRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<DeleteIssueArticleRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteIssueArticleRequest> HandleAsync(DeleteIssueArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await articleRepository.GetIssueArticle(command.PeriodicalId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber,
            command.SequenceNumber, cancellationToken);
        if (issue is not null)
        {
            var contents = await articleRepository.GetIssueArticleContents(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

            foreach (var content in contents)  
            {
                await commandProcessor.SendAsync(new DeleteIssueArticleContentRequest(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, content.Language), cancellationToken: cancellationToken);
            }
            await articleRepository.DeleteIssueArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
