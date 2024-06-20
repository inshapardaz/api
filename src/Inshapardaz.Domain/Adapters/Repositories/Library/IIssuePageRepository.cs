using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library;

public interface IIssuePageRepository
{
    Task<Page<IssuePageModel>> GetPagesByIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int pageNumber, int pageSize, EditingStatus statusFilter, AssignmentFilter assignmentFilter, AssignmentFilter reviewerAssignmentFilter, int? accountId, CancellationToken cancellationToken);
    Task<IssuePageModel> GetPageBySequenceNumber(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);
    Task<IssuePageModel> AddPage(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, long? fileId, long? imageId, long? chapterNumber, EditingStatus status, CancellationToken cancellationToken);
    Task<IssuePageModel> UpdatePage(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, long? fileId, long? imageId, long? chapterNumber, EditingStatus status, CancellationToken cancellationToken);
    Task UpdatePageSequenceNumber(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int oldSequenceNumber, int newSequenceNumber, CancellationToken cancellationToken);
    Task<Page<IssuePageModel>> GetPagesByUser(int libraryId, int accountId, EditingStatus statusFilter, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<int> GetLastPageNumberForIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken);
    Task DeletePageImage(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);
    Task DeletePage(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);
    Task<IssuePageModel> UpdatePageImage(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, long imageId, CancellationToken cancellationToken);
    Task<IssuePageModel> UpdateWriterAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? accountId, CancellationToken cancellationToken);
    Task<IssuePageModel> UpdateReviewerAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? accountId, CancellationToken cancellationToken);
}
