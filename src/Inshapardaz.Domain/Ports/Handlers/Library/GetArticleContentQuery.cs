using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories.Library;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetArticleContentQuery : LibraryBaseQuery<ArticleContentModel>
    {
        public GetArticleContentQuery(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, string language)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            ArticleId = articleId;
            Language = language;
        }

        public int PeriodicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
        public int ArticleId { get; }
        public string Language { get; set; }
    }

    public class GetArticleContentQueryHandler : QueryHandlerAsync<GetArticleContentQuery, ArticleContentModel>
    {
        private readonly IArticleRepository _articleRepository;

        public GetArticleContentQueryHandler(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public override async Task<ArticleContentModel> ExecuteAsync(GetArticleContentQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _articleRepository.GetArticleContentById(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ArticleId, command.Language, cancellationToken);
        }
    }
}
