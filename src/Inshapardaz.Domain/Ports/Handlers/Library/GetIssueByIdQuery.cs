using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetIssueByIdQuery : LibraryBaseQuery<IssueModel>
    {
        public GetIssueByIdQuery(int libraryId, int periodicalId, int volumeNumber, int issueNumber)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
        }

        public int PeriodicalId { get; private set; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
    }

    public class GetIssueByIdQueryHandler : QueryHandlerAsync<GetIssueByIdQuery, IssueModel>
    {
        private readonly IIssueRepository _issueRepository;

        public GetIssueByIdQueryHandler(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public override async Task<IssueModel> ExecuteAsync(GetIssueByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        }
    }
}
