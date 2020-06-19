using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteIssueRequest : LibraryAuthorisedCommand
    {
        public DeleteIssueRequest(ClaimsPrincipal claims, int libraryId, int periodicalId, int issueId)
            : base(claims, libraryId)
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

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<DeleteIssueRequest> HandleAsync(DeleteIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _issueRepository.DeleteIssue(command.PeriodicalId, command.IssueId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
