using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class AssignArticleToUserRequest(int libraryId, int articleId, int? accountId) : LibraryBaseCommand(libraryId)
{
    public ArticleModel Result { get; set; }
    public int ArticleId { get; set; } = articleId;
    public int? AccountId { get; private set; } = accountId;
}

public class AssignArticleToUserRequestHandler(
    IAccountRepository accountRepository,
    IArticleRepository articleRepository,
    IUserHelper userHelper)
    : RequestHandlerAsync<AssignArticleToUserRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AssignArticleToUserRequest> HandleAsync(AssignArticleToUserRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var accountToAssignTo = command.AccountId ?? userHelper.Account.Id;
        if (!userHelper.IsAdmin)
        {
            var account = await accountRepository.GetLibraryAccountById(command.LibraryId, accountToAssignTo, cancellationToken);
            if (account.Role != Role.LibraryAdmin && account.Role != Role.Writer)
            {
                throw new BadRequestException("user cannot be assigned article");
            }
        }

        var article = await articleRepository.GetArticle(command.LibraryId, command.ArticleId, cancellationToken);
        if (article == null)
        {
            throw new BadRequestException();
        }

        if (article.Status == EditingStatus.Available || article.Status == EditingStatus.Typing)
        {
            command.Result = await articleRepository.UpdateWriterAssignment(command.LibraryId, command.ArticleId, accountToAssignTo, cancellationToken);
        }
        else if (article.Status == EditingStatus.Typed || article.Status == EditingStatus.InReview)
        {
            command.Result = await articleRepository.UpdateReviewerAssignment(command.LibraryId, command.ArticleId, accountToAssignTo, cancellationToken);
        }
        else
        {
            throw new BadRequestException("Article does not allow it to be assigned");
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
