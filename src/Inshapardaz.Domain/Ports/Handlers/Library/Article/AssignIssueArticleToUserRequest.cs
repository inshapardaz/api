using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Article
{
    public class AssignArticleToUserRequest : LibraryBaseCommand
    {
        public AssignArticleToUserRequest(int libraryId, int articleId, int? accountId, bool isAdmin = false)
        : base(libraryId)
        {
            ArticleId = articleId;
            AccountId = accountId;
            IsAdmin = isAdmin;
        }

        public ArticleModel Result { get; set; }
        public int ArticleId { get; set; }
        public int? AccountId { get; private set; }
        public bool IsAdmin { get; }
    }

    public class AssignArticleToUserRequestHandler : RequestHandlerAsync<AssignArticleToUserRequest>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IArticleRepository _articleRepository;

        public AssignArticleToUserRequestHandler(IAccountRepository accountRepository,
                                         IArticleRepository articleRepository)
        {
            _accountRepository = accountRepository;
            _articleRepository = articleRepository;
        }

        public override async Task<AssignArticleToUserRequest> HandleAsync(AssignArticleToUserRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (!command.IsAdmin)
            {
                var account = await _accountRepository.GetLibraryAccountById(command.LibraryId, command.AccountId.Value, cancellationToken);
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
                command.Result = await _articleRepository.UpdateWriterAssignment(command.LibraryId, command.ArticleId, command.AccountId, cancellationToken);
            }
            else if (article.Status == EditingStatus.Typed || article.Status == EditingStatus.InReview)
            {
                command.Result = await _articleRepository.UpdateReviewerAssignment(command.LibraryId, command.ArticleId, command.AccountId, cancellationToken);
            }
            else
            {
                throw new BadRequestException("Article does not allow it to be assigned");
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
