using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;

public class AssignIssueArticleToUserRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int sequenceNumber,
    int? accountId,
    bool isAdmin = false)
    : LibraryBaseCommand(libraryId)
{
    public IssueArticleModel Result { get; set; }
    public int PeriodicalId { get; set; } = periodicalId;
    public int VolumeNumber { get; set; } = volumeNumber;
    public int IssueNumber { get; set; } = issueNumber;
    public int SequenceNumber { get; set; } = sequenceNumber;
    public int? AccountId { get; private set; } = accountId;
    public bool IsAdmin { get; } = isAdmin;
}

public class AssignArticleToUserRequestHandler(
    IAccountRepository accountRepository,
    IIssueArticleRepository articleRepository)
    : RequestHandlerAsync<AssignIssueArticleToUserRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AssignIssueArticleToUserRequest> HandleAsync(AssignIssueArticleToUserRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (!command.IsAdmin)
        {
            var account = await accountRepository.GetLibraryAccountById(command.LibraryId, command.AccountId.Value, cancellationToken);
            if (account == null ||  account.Role != Role.LibraryAdmin && account.Role != Role.Writer)
            {
                throw new BadRequestException("user cannot be assigned article");
            }
        }

        var article = await articleRepository.GetIssueArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        if (article == null)
        {
            throw new BadRequestException();
        }

        if (article.Status == EditingStatus.Available || article.Status == EditingStatus.Typing)
        {
            command.Result = await articleRepository.UpdateWriterAssignment(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.AccountId, cancellationToken);
        }
        else if (article.Status == EditingStatus.Typed || article.Status == EditingStatus.InReview)
        {
            command.Result = await articleRepository.UpdateReviewerAssignment(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.AccountId, cancellationToken);
        }
        else
        {
            throw new BadRequestException("Article does not allow it to be assigned");
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
