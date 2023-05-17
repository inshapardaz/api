using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Periodical.Issue
{
    public class GetIssuesQuery : LibraryBaseQuery<Page<IssueModel>>
    {
        public GetIssuesQuery(int libraryId, int periodicalId, int pageNumber, int pageSize)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PeriodicalId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
        public IssueFilter Filter { get; set; }
        public IssueSortByType SortBy { get; set; }
        public SortDirection SortDirection { get; set; }
    }

    public class GetIssuesRequestHandler : QueryHandlerAsync<GetIssuesQuery, Page<IssueModel>>
    {
        private readonly IPeriodicalRepository _periodicalRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IIssueRepository _issueRepository;

        public GetIssuesRequestHandler(IIssueRepository issueRepository, IPeriodicalRepository periodicalRepository, IFileRepository fileRepository)
        {
            _issueRepository = issueRepository;
            _periodicalRepository = periodicalRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<Page<IssueModel>> ExecuteAsync(GetIssuesQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var periodical = await _periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);
            if (periodical == null) return null;
            var issues = await _issueRepository.GetIssues(command.LibraryId, command.PeriodicalId, command.PageNumber, command.PageSize, command.Filter, command.SortBy, command.SortDirection, cancellationToken);
            foreach (var issue in issues.Data)
            {
                if (issue != null && issue.ImageUrl == null && issue.ImageId.HasValue)
                {
                    issue.ImageUrl = await ImageHelper.TryConvertToPublicFile(issue.ImageId.Value, _fileRepository, cancellationToken);
                }

                var contents = await _issueRepository.GetIssueContents(command.LibraryId, issue.PeriodicalId, issue.VolumeNumber, issue.IssueNumber, cancellationToken);

                issue.Contents = contents.ToList();
            }

            return issues;
        }
    }
}
