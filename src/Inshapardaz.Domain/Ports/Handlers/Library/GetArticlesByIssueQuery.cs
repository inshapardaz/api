using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetArticlesByIssueQuery : LibraryBaseQuery<IEnumerable<ArticleModel>>
    {
        public GetArticlesByIssueQuery(int libraryId, int periodicalId, int volumeNumber, int issueNumber)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
        }

        public int PeriodicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
    }

    public class GetArticlesByIssueQuerytHandler : QueryHandlerAsync<GetArticlesByIssueQuery, IEnumerable<ArticleModel>>
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IArticleRepository _articleRepository;

        public GetArticlesByIssueQuerytHandler(IIssueRepository issueRepository, IArticleRepository articleRepository)
        {
            _issueRepository = issueRepository;
            _articleRepository = articleRepository;
        }

        public override async Task<IEnumerable<ArticleModel>> ExecuteAsync(GetArticlesByIssueQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken); 
            if (issue == null)
            {
                throw new BadRequestException();
            }

            return await _articleRepository.GetArticlesByIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        }
    }
}
