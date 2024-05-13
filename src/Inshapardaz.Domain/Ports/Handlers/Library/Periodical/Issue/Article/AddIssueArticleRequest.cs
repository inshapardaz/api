using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Periodical.Issue.Article
{
    public class AddIssueArticleRequest : LibraryBaseCommand
    {
        public AddIssueArticleRequest(int libraryId, int peridicalId, int volumeNumber, int issueNumber, IssueArticleModel article)
            : base(libraryId)
        {
            PeridicalId = peridicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            Article = article;
        }

        public IssueArticleModel Result { get; set; }
        public int PeridicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
        public IssueArticleModel Article { get; }
    }

    public class AddArticleRequestHandler : RequestHandlerAsync<AddIssueArticleRequest>
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IIssueArticleRepository _articleRepository;

        public AddArticleRequestHandler(IIssueRepository issueRepository, IIssueArticleRepository articleRepository)
        {
            _issueRepository = issueRepository;
            _articleRepository = articleRepository;
        }

        [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
        public override async Task<AddIssueArticleRequest> HandleAsync(AddIssueArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
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
