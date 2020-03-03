using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddIssueRequest : LibraryAuthorisedCommand
    {
        public AddIssueRequest(ClaimsPrincipal claims, int libraryId, int periodicalId, IssueModel issue)
            : base(claims, libraryId)
        {
            PeriodicalId = periodicalId;
            Issue = issue;
        }

        public int PeriodicalId { get; private set; }

        public IssueModel Issue { get; }

        public IssueModel Result { get; set; }
    }

    public class AddIssueRequestHandler : RequestHandlerAsync<AddIssueRequest>
    {
        private readonly IIssueRepository _issueRepository;

        public AddIssueRequestHandler(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<AddIssueRequest> HandleAsync(AddIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _issueRepository.AddIssue(command.LibraryId, command.PeriodicalId, command.Issue, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
