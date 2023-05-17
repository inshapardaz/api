using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Periodical.Issue.Page
{
    public class GetIssuePagesQuery : LibraryBaseQuery<Page<IssuePageModel>>
    {
        public GetIssuePagesQuery(int libraryId,
                                int periodicalId,
                                int volumeNumber,
                                int issueNumber,
                                int pageNumber,
                                int pageSize)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PeriodicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public EditingStatus StatusFilter { get; set; }
        public AssignmentFilter AssignmentFilter { get; set; }
        public int? AccountId { get; set; }
        public AssignmentFilter ReviewerAssignmentFilter { get; set; }
    }

    public class GetIssuePagesQueryHandler : QueryHandlerAsync<GetIssuePagesQuery, Page<IssuePageModel>>
    {
        private readonly IIssuePageRepository _issuePageRepository;

        public GetIssuePagesQueryHandler(IIssuePageRepository issuePageRepository)
        {
            _issuePageRepository = issuePageRepository;
        }

        public override async Task<Page<IssuePageModel>> ExecuteAsync(GetIssuePagesQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _issuePageRepository.GetPagesByIssue(query.LibraryId, query.PeriodicalId, query.VolumeNumber, query.IssueNumber, query.PageNumber, query.PageSize, query.StatusFilter, query.AssignmentFilter, query.ReviewerAssignmentFilter, query.AccountId, cancellationToken);
        }
    }
}
