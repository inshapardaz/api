using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetIssueByIdRequest : RequestBase
    {
        public GetIssueByIdRequest(int periodicalId, int issueId)
        {
            PeriodicalId = periodicalId;
            IssueId = issueId;
        }

        public Issue Result { get; set; }

        public int PeriodicalId { get; private set; }

        public int IssueId { get; }
    }

    public class GetIssueByIdRequestHandler : RequestHandlerAsync<GetIssueByIdRequest>
    {
        private readonly IIssueRepository _issueRepository;

        public GetIssueByIdRequestHandler(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public override async Task<GetIssueByIdRequest> HandleAsync(GetIssueByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _issueRepository.GetIssueById(command.PeriodicalId, command.IssueId, cancellationToken);
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

