using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Library;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Ports.Database.Repositories.Library
{
    public class PeriodicalRepository : IPeriodicalRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public PeriodicalRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Page<Periodical>> GetPeriodicals(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var count = await _databaseContext.Periodical.CountAsync(cancellationToken);
            var data = await _databaseContext.Periodical
                             .Paginate(pageNumber, pageSize)
                             .Select(a => a.Map<Entities.Library.Periodical, Periodical>())
                             .ToListAsync(cancellationToken);

            return new Page<Periodical>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }

        public async Task<Page<Periodical>> SearchPeriodicals(string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var periodicals = _databaseContext.Periodical.Where(b => b.Title.Contains(query));
            var count = await periodicals.CountAsync(cancellationToken);
            var data = await periodicals
                             .Paginate(pageNumber, pageSize)
                             .Select(a => a.Map<Entities.Library.Periodical, Periodical>())
                             .ToListAsync(cancellationToken);

            return new Page<Periodical>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }

        public async Task<Periodical> GetPeriodicalById(int periodicalId, CancellationToken cancellationToken)
        {
            var periodical = await _databaseContext.Periodical
                                             .SingleOrDefaultAsync(t => t.Id == periodicalId,
                                                                     cancellationToken);
            return periodical.Map<Entities.Library.Periodical, Periodical>();
        }

        public async Task<Periodical> AddPeriodical(Periodical periodical, CancellationToken cancellationToken)
        {
            var item = periodical.Map<Periodical, Entities.Library.Periodical>();

            _databaseContext.Periodical.Add(item);

            await _databaseContext.SaveChangesAsync(cancellationToken);
            return item.Map<Entities.Library.Periodical, Periodical>();
        }

        public async Task UpdatePeriodical(Periodical periodical, CancellationToken cancellationToken)
        {
            var existingEntity = await _databaseContext.Periodical
                                                        .SingleOrDefaultAsync(g => g.Id == periodical.Id,
                                                                              cancellationToken);

            if (existingEntity == null)
            {
                throw new NotFoundException();
            }

            existingEntity.Title = periodical.Title;
            existingEntity.Description = periodical.Description;
            existingEntity.CategoryId = periodical.CategoryId;

            if (periodical.ImageId > 0)
            {
                existingEntity.ImageId = periodical.ImageId;
            }

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeletePeriodical(int periodicalId, CancellationToken cancellationToken)
        {
            var periodical = await _databaseContext.Periodical.SingleOrDefaultAsync(g => g.Id == periodicalId, cancellationToken);

            if (periodical != null)
            {
                _databaseContext.Periodical.Remove(periodical);
                await _databaseContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
