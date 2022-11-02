using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddArticleRequest : LibraryBaseCommand
    {
        public AddArticleRequest(int libraryId, int peridicalId, int volumeNumber, int issueNumber, ArticleModel article)
            : base(libraryId)
        {
            PeridicalId = peridicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            Article = article;
        }

        public ArticleModel Result { get; set; }
        public int PeridicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
        public ArticleModel Article { get; }
    }

    public class AddArticleRequestHandler : RequestHandlerAsync<AddArticleRequest>
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IArticleRepository _articleRepository;

        public AddArticleRequestHandler(IIssueRepository issueRepository, IArticleRepository articleRepository)
        {
            _issueRepository = issueRepository;
            _articleRepository = articleRepository;
        }

        public override async Task<AddArticleRequest> HandleAsync(AddArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeridicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
            if (issue == null)
            {
                throw new BadRequestException();
            }

            command.Article.SequenceNumber = issue.ArticleCount + 1;
            command.Result = await _articleRepository.AddArticle(command.LibraryId, command.PeridicalId, command.VolumeNumber, command.IssueNumber, command.Article, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
