using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Periodical.Issue
{
    public class GetIssuesYearQuery : LibraryBaseQuery<IEnumerable<(int Year, int count)>>
    {
        public GetIssuesYearQuery(int libraryId, int periodicalId)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
        }

        public int PeriodicalId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
        public SortDirection SortDirection { get; set; }
        public AssignmentStatus AssignmentStatus { get; set; }
    }

    public class GetIssuesYearQueryHandler : QueryHandlerAsync<GetIssuesYearQuery, IEnumerable<(int Year, int count)>>
    {
        private readonly IPeriodicalRepository _periodicalRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IIssueRepository _issueRepository;

        public GetIssuesYearQueryHandler(IIssueRepository issueRepository, IPeriodicalRepository periodicalRepository, IFileRepository fileRepository)
        {
            _issueRepository = issueRepository;
            _periodicalRepository = periodicalRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<IEnumerable<(int Year, int count)>> ExecuteAsync(GetIssuesYearQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var periodical = await _periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);
            if (periodical == null) return null;
            var issues = await _issueRepository.GetIssuesYear(command.LibraryId, command.PeriodicalId, command.AssignmentStatus, command.SortDirection, cancellationToken);

            return issues;
        }
    }
}
