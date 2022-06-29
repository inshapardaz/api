using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library
{
    public interface IBookPageRepository
    {
        Task<BookPageModel> GetPageBySequenceNumber(int libraryId, int bookId, int sequenceNumber, CancellationToken cancellationToken);

        Task<int> GetLastPageNumberForBook(int libraryId, int bookId, CancellationToken cancellationToken);

        Task<BookPageModel> AddPage(int libraryId, int bookId, int sequenceNumber, string text, int imageId, int? chapterId, CancellationToken cancellationToken);

        Task<BookPageModel> UpdatePage(int libraryId, int bookId, int sequenceNumber, string text, int imageId, EditingStatus status, int? chapterId, CancellationToken cancellationToken);

        Task DeletePage(int libraryId, int bookId, int sequenceNumber, CancellationToken cancellationToken);

        Task<BookPageModel> UpdatePageImage(int libraryId, int bookId, int sequenceNumber, int imageId, CancellationToken cancellationToken);

        Task<IEnumerable<BookPageModel>> GetAllPagesByBook(int libraryId, int bookId, CancellationToken cancellationToken);

        Task DeletePageImage(int libraryId, int bookId, int sequenceNumber, CancellationToken cancellationToken);
        Task<IEnumerable<UserPageSummaryItem>> GetUserPageSummary(int libraryId, int accountId, CancellationToken cancellationToken);
        Task<int> GetPageCount(int libraryId, int bookId, int oldSequenceNumber, CancellationToken cancellationToken);
        Task<Page<BookPageModel>> GetPagesByBook(int libraryId, int bookId, int pageNumber, int pageSize, EditingStatus status, AssignmentFilter assignmentFilter, AssignmentFilter reviewerAssignmentFilter, int? assignedTo, CancellationToken cancellationToken);
        Task<Page<BookPageModel>> GetPagesByUser(int libraryId, int assignedTo, EditingStatus statusFilter, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<BookPageModel>> GetPagesByBookChapter(int libraryId, int bookId, int chapterId, CancellationToken cancellationToken);

        Task<BookPageModel> UpdateWriterAssignment(int libraryId, int bookId, int sequenceNumber, int? assignedAccountId, CancellationToken cancellationToken);
        Task UpdatePageSequenceNumber(int libraryId, int bookId, int oldSequenceNumber, int newSequenceNumber, CancellationToken cancellationToken);
        Task<BookPageModel> UpdateReviewerAssignment(int libraryId, int bookId, int sequenceNumber, int? assignedAccountId, CancellationToken cancellationToken);
    }
}
