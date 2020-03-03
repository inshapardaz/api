using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IIssueRepository
    {
        Task<IssueModel> GetIssueById(int libraryId, int periodicalId, int issueId, CancellationToken cancellationToken);

        Task<Page<IssueModel>> GetIssues(int libraryId, int periodicalId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IssueModel> AddIssue(int libraryId, int periodicalId, IssueModel issue, CancellationToken cancellationToken);

        Task UpdateIssue(int libraryId, int periodicalId, IssueModel issue, CancellationToken cancellationToken);

        Task DeleteIssue(int libraryId, int issueId, CancellationToken cancellationToken);
    }
}
