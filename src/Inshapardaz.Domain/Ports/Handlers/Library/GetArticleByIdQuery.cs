using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetArticleByIdQuery : LibraryBaseQuery<ArticleModel>
    {
        public GetArticleByIdQuery(int libraryId, int periodicalId, int issueId, int articleId, int? userId)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            IssueId = issueId;
            ArticleId = articleId;
            UserId = userId;
        }

        public int PeriodicalId { get; }
        public int IssueId { get; }
        public int ArticleId { get; }
        public int? UserId { get; set; }
    }

    public class GetArticleByIdQueryHandler : QueryHandlerAsync<GetArticleByIdQuery, ArticleModel>
    {
        private readonly IArticleRepository _articleRepository;

        public GetArticleByIdQueryHandler(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public override async Task<ArticleModel> ExecuteAsync(GetArticleByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _articleRepository.GetArticleById(command.LibraryId, command.PeriodicalId, command.IssueId, command.ArticleId, cancellationToken);
        }
    }
}
