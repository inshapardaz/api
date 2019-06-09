using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteIssueRequest : RequestBase
    {
        public DeleteIssueRequest(int periodicalId, int issueId)
        {
            PeriodicalId = periodicalId;
            IssueId = issueId;
        }

        public int PeriodicalId { get; private set; }

        public int IssueId { get; }
    }

    public class DeleteIssueRequestHandler : RequestHandlerAsync<DeleteIssueRequest>
    {
        private readonly IIssueRepository _issueRepository;

        public DeleteIssueRequestHandler(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public override async Task<DeleteIssueRequest> HandleAsync(DeleteIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _issueRepository.DeleteIssue(command.PeriodicalId, command.IssueId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}