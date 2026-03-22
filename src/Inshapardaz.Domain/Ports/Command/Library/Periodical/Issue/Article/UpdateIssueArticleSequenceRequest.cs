using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;

public class UpdateIssueArticleSequenceRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    IEnumerable<IssueArticleModel> articles)
    : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public IEnumerable<IssueArticleModel> Articles { get; } = articles;

    public IEnumerable<IssueArticleModel> Result { get; set; }
}

public class UpdateIssueArticleSequenceRequestHandler(IIssueArticleRepository articleRepository)
    : RequestHandlerAsync<UpdateIssueArticleSequenceRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssueArticleSequenceRequest> HandleAsync(UpdateIssueArticleSequenceRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var articles = await articleRepository.GetIssueArticlesByIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

        if (articles != null)
        {
            foreach (var a1 in command.Articles)
            {
                var a2 = articles.SingleOrDefault(x => x.Id == a1.Id);
                if (a2 == null)
                {
                    throw new BadRequestException("Resource out of date.");
                }
                a2.SequenceNumber = a1.SequenceNumber;
            }

            await articleRepository.UpdateArticleSequence(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, articles, cancellationToken);
            command.Result = await articleRepository.GetIssueArticlesByIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
