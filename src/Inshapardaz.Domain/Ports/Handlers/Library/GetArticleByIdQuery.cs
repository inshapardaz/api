using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetArticleByIdQuery : LibraryBaseQuery<ArticleModel>
    {
        public GetArticleByIdQuery(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            ArticleId = articleId;
        }

        public int PeriodicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
        public int ArticleId { get; }
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
            return await _articleRepository.GetArticleById(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ArticleId, cancellationToken);
        }
    }
}
