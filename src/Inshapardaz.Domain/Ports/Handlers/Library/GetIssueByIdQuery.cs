using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetIssueByIdQuery : LibraryBaseQuery<IssueModel>
    {
        public GetIssueByIdQuery(int libraryId, int periodicalId, int issueId)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            IssueId = issueId;
        }

        public int PeriodicalId { get; private set; }

        public int IssueId { get; }
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
            return await _issueRepository.GetIssueById(command.LibraryId, command.PeriodicalId, command.IssueId, cancellationToken);
        }
    }
}
