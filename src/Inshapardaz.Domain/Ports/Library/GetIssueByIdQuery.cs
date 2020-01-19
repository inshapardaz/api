using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetIssueByIdQuery : IQuery<Issue>
    {
        public GetIssueByIdQuery(int periodicalId, int issueId)
        {
            PeriodicalId = periodicalId;
            IssueId = issueId;
        }

        public int PeriodicalId { get; private set; }

        public int IssueId { get; }
    }

    public class GetIssueByIdQueryHandler : QueryHandlerAsync<GetIssueByIdQuery, Issue>
    {
        private readonly IIssueRepository _issueRepository;

        public GetIssueByIdQueryHandler(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public override async Task<Issue> ExecuteAsync(GetIssueByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _issueRepository.GetIssueById(command.PeriodicalId, command.IssueId, cancellationToken);
        }
    }
}

