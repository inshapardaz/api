using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class AssignArticleToUserRequest : LibraryBaseCommand
{
    public AssignArticleToUserRequest(int libraryId, int articleId, int? accountId)
    : base(libraryId)
    {
        ArticleId = articleId;
        AccountId = accountId;
    }

    public ArticleModel Result { get; set; }
    public int ArticleId { get; set; }
    public int? AccountId { get; private set; }
}

public class AssignArticleToUserRequestHandler : RequestHandlerAsync<AssignArticleToUserRequest>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IUserHelper _userHelper;

    public AssignArticleToUserRequestHandler(IAccountRepository accountRepository,
                                     IArticleRepository articleRepository,
                                     IUserHelper userHelper)
    {
        _accountRepository = accountRepository;
        _articleRepository = articleRepository;
        _userHelper = userHelper;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AssignArticleToUserRequest> HandleAsync(AssignArticleToUserRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var accountToAssignTo = command.AccountId ?? _userHelper.Account.Id;
        if (!_userHelper.IsAdmin)
        {
            var account = await _accountRepository.GetLibraryAccountById(command.LibraryId, accountToAssignTo, cancellationToken);
            if (account.Role != Role.LibraryAdmin && account.Role != Role.Writer)
            {
                throw new BadRequestException("user cannot be assigned article");
            }
        }

        var article = await _articleRepository.GetArticle(command.LibraryId, command.ArticleId, cancellationToken);
        if (article == null)
        {
            throw new BadRequestException();
        }

        if (article.Status == EditingStatus.Available || article.Status == EditingStatus.Typing)
        {
            command.Result = await _articleRepository.UpdateWriterAssignment(command.LibraryId, command.ArticleId, accountToAssignTo, cancellationToken);
        }
        else if (article.Status == EditingStatus.Typed || article.Status == EditingStatus.InReview)
        {
            command.Result = await _articleRepository.UpdateReviewerAssignment(command.LibraryId, command.ArticleId, accountToAssignTo, cancellationToken);
        }
        else
        {
            throw new BadRequestException("Article does not allow it to be assigned");
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
