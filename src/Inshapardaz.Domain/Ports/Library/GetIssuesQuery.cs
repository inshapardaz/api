using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetIssuesQuery :  IQuery<Page<Issue>>
    {
        public GetIssuesQuery(int periodicalId, int pageNumber, int pageSize)
        {
            PeriodicalId = periodicalId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PeriodicalId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
    }

    public class GetIssuesRequestHandler : QueryHandlerAsync<GetIssuesQuery, Page<Issue>>
    {
        private readonly IIssueRepository _issueRepository;

        public GetIssuesRequestHandler(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public override async Task<Page<Issue>> ExecuteAsync(GetIssuesQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _issueRepository.GetIssues(command.PeriodicalId, command.PageNumber, command.PageSize, cancellationToken);
        }
    }
}

