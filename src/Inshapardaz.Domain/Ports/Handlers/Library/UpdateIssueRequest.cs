using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateIssueRequest : LibraryBaseCommand
    {
        public UpdateIssueRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IssueModel issue)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            Issue = issue;
        }

        public int PeriodicalId { get; private set; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; set; }

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

        public override async Task<UpdateIssueRequest> HandleAsync(UpdateIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

            if (result == null)
            {
                var Issue = command.Issue;
                Issue.Id = default(int);
                command.Result.Issue = await _issueRepository.AddIssue(command.LibraryId, command.PeriodicalId, Issue, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                result.IssueNumber = command.Issue.IssueNumber;
                result.VolumeNumber = command.Issue.VolumeNumber;
                result.IssueDate = command.Issue.IssueDate;

                await _issueRepository.UpdateIssue(command.LibraryId, command.PeriodicalId, result, cancellationToken);
                command.Result.Issue = command.Issue;
            }
            command.Issue.PeriodicalId = command.PeriodicalId;

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
