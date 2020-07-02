using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetIssuesQuery : LibraryBaseQuery<Page<IssueModel>>
    {
        public GetIssuesQuery(int libraryId, int periodicalId, int pageNumber, int pageSize)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PeriodicalId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
    }

    public class GetIssuesRequestHandler : QueryHandlerAsync<GetIssuesQuery, Page<IssueModel>>
    {
        private readonly IIssueRepository _issueRepository;

        public GetIssuesRequestHandler(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public override async Task<Page<IssueModel>> ExecuteAsync(GetIssuesQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _issueRepository.GetIssues(command.LibraryId, command.PeriodicalId, command.PageNumber, command.PageSize, cancellationToken);
        }
    }
}
