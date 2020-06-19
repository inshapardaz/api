using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateIssueRequest : LibraryAuthorisedCommand
    {
        public UpdateIssueRequest(ClaimsPrincipal claims, int libraryId, int periodicalId, int issueId, IssueModel issue)
            : base(claims, libraryId)
        {
            PeriodicalId = periodicalId;
            IssueId = issueId;
            Issue = issue;
        }

        public int PeriodicalId { get; private set; }

        public int IssueId { get; set; }

        public IssueModel Issue { get; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public IssueModel Issue { get; set; }

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

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<UpdateIssueRequest> HandleAsync(UpdateIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _issueRepository.GetIssueById(command.LibraryId, command.PeriodicalId, command.IssueId, cancellationToken);

            if (result == null)
            {
                var Issue = command.Issue;
                Issue.Id = default(int);
                command.Result.Issue = await _issueRepository.AddIssue(command.LibraryId, command.PeriodicalId, Issue, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _issueRepository.UpdateIssue(command.LibraryId, command.PeriodicalId, command.Issue, cancellationToken);
                command.Result.Issue = command.Issue;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
