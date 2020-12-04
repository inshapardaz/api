using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddIssueRequest : LibraryBaseCommand
    {
        public AddIssueRequest(int libraryId, int periodicalId, IssueModel issue)
            : base(libraryId)
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

        public override async Task<AddIssueRequest> HandleAsync(AddIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _issueRepository.AddIssue(command.LibraryId, command.PeriodicalId, command.Issue, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
