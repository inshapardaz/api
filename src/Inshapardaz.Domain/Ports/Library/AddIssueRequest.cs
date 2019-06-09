using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddIssueRequest : RequestBase
    {
        public AddIssueRequest(int periodicalId, Issue issue)
        {
            PeriodicalId = periodicalId;
            Issue = issue;
        }

        public int PeriodicalId { get; private set; }

        public Issue Issue { get; }

        public Issue Result { get; set; }
    }

    public class AddIssueRequestHandler : RequestHandlerAsync<AddIssueRequest>
    {
        private readonly IIssueRepository _issueRepository;

        public AddIssueRequestHandler(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public override async Task<AddIssueRequest> HandleAsync(AddIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result= await _issueRepository.AddIssue(command.PeriodicalId, command.Issue, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}
