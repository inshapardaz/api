using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Library;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Ports.Database.Repositories.Library
{
    public class IssueRepository : IIssueRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public IssueRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Page<Issue>> GetIssues(int periodicalId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _databaseContext.Issue.Where(i => i.PeriodicalId == periodicalId);
            var count = await query.CountAsync(cancellationToken);
            var data = await query
                             .Paginate(pageNumber, pageSize)
                             .Select(a => a.Map())
                             .ToListAsync(cancellationToken);

            return new Page<Issue>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }

        public async Task<Issue> GetIssueById(int periodicalId, int issueId, CancellationToken cancellationToken)
        {
            var periodical = await _databaseContext.Issue
                                             .SingleOrDefaultAsync(t => t.Id == issueId, cancellationToken);
            return periodical.Map();
        }

        public async Task<Issue> AddIssue(int periodicalId, Issue issue, CancellationToken cancellationToken)
        {
            var periodical = await _databaseContext.Periodical
                                             .SingleOrDefaultAsync(t => t.Id == periodicalId,
                                                                   cancellationToken);
            if (periodical == null)
            {
                throw new NotFoundException();
            }

            var item = issue.Map();
            item.Periodical = periodical;
            _databaseContext.Issue.Add(item);

            await _databaseContext.SaveChangesAsync(cancellationToken);
            return item.Map();
        }

        public async Task UpdateIssue(int periodicalId, Issue issue, CancellationToken cancellationToken)
        {
            var periodical = await _databaseContext.Periodical
                                             .SingleOrDefaultAsync(t => t.Id == periodicalId,
                                                                   cancellationToken);
            if (periodical == null)
            {
                throw new NotFoundException();
            }

            var existingEntity = await _databaseContext.Issue
                                                        .SingleOrDefaultAsync(g => g.Id == periodicalId, cancellationToken);

            if (existingEntity == null)
            {
                throw new NotFoundException();
            }

            existingEntity.IssueDate = issue.IssueDate;
            existingEntity.IssueNumber = issue.IssueNumber;
            existingEntity.VolumeNumber = issue.VolumeNumber;
            existingEntity.PeriodicalId = periodicalId;

            if (issue.ImageId > 0)
            {
                existingEntity.ImageId = issue.ImageId;
            }

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteIssue(int periodicalId, int issueId, CancellationToken cancellationToken)
        {
            var issue = await _databaseContext.Issue.SingleOrDefaultAsync(g => g.Id == issueId, cancellationToken);

            if (issue != null)
            {
                _databaseContext.Issue.Remove(issue);
                await _databaseContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
