using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library;

public interface IIssueRepository
{
    Task<IssueModel> GetIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken);

    Task<Page<IssueModel>> GetIssues(int libraryId, int periodicalId, int pageNumber, int pageSize, IssueFilter filter, IssueSortByType sortBy, SortDirection sortDirection, CancellationToken cancellationToken);

    Task<IEnumerable<(int Year, int count)>> GetIssuesYear(int libraryId, int periodicalId, AssignmentStatus assignmentStatus, SortDirection sortDirection, CancellationToken cancellationToken);

    Task<IssueModel> AddIssue(int libraryId, int periodicalId, IssueModel issue, CancellationToken cancellationToken);

    Task UpdateIssue(int libraryId, int periodicalId, IssueModel issue, CancellationToken cancellationToken);

    Task DeleteIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken);

    Task<IssueContentModel> AddIssueContent(int libraryId, IssueContentModel model, CancellationToken cancellationToken);

    Task<IEnumerable<IssueContentModel>> GetIssueContents(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken);

    Task<IssueContentModel> GetIssueContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, long contentId, CancellationToken cancellationToken);

    Task UpdateIssueContentUrl(int libraryId, int periodicalId, int volumeNumber, int issueNumber, long contentid, string url, CancellationToken cancellationToken);

    Task DeleteIssueContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, long contentId, CancellationToken cancellationToken);

    Task<IssueContentModel> UpdateIssueContent(int libraryId, IssueContentModel model, CancellationToken cancellationToken);
    Task<IEnumerable<PageSummaryModel>> GetIssuePageSummary(int libraryId, int[] issues, CancellationToken cancellationToken);
}
