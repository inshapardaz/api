using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateIssueRequest : RequestBase
    {
        public UpdateIssueRequest(int periodicalId, int issueId, Issue issue)
        {
            PeriodicalId = periodicalId;
            IssueId = issueId;
            Issue = issue;
        }

        public int PeriodicalId { get; private set; }

        public int IssueId { get; set; }

        public Issue Issue { get; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public Issue Issue { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateIssueRequestHandler : RequestHandlerAsync<UpdateIssueRequest>
    {
        private readonly IIssueRepository _issueRepository;

        public UpdateIssueRequestHandler(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public override async Task<UpdateIssueRequest> HandleAsync(UpdateIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _issueRepository.GetIssueById(command.PeriodicalId, command.IssueId, cancellationToken);

            if (result == null)
            {
                var Issue = command.Issue;
                Issue.Id = default(int);
                command.Result.Issue =  await  _issueRepository.AddIssue(command.PeriodicalId, Issue, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _issueRepository.UpdateIssue(command.PeriodicalId, command.Issue, cancellationToken);
                command.Result.Issue = command.Issue;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}