using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IIssueRepository
    {
        Task<Issue> GetIssueById(int periodicalId, int issueId, CancellationToken cancellationToken);
        Task<Page<Issue>> GetIssues(int periodicalId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Issue> AddIssue(int periodicalId, Issue issue, CancellationToken cancellationToken);
        Task UpdateIssue(int periodicalId, Issue issue, CancellationToken cancellationToken);
        Task DeleteIssue(int periodicalId, int issueId, CancellationToken cancellationToken);
    }
}
