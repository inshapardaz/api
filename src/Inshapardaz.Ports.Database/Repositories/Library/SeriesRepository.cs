using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Library;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Ports.Database.Repositories.Library
{
    public class SeriesRepository : ISeriesRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public SeriesRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Series> AddSeries(Series series, CancellationToken cancellationToken)
        {
            var item = series.Map<Series, Entities.Library.Series>();
            _databaseContext.Series.Add(item);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            return item.Map<Entities.Library.Series, Series>();
        }

        public async Task UpdateSeries(Series series, CancellationToken cancellationToken)
        {
            var existingEntity = await _databaseContext.Series
                                                       .SingleOrDefaultAsync(g => g.Id == series.Id,
                                                                             cancellationToken);

            if (existingEntity == null)
            {
                throw new NotFoundException();
            }

            existingEntity.Name = series.Name;
            existingEntity.Description = series.Description;

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteSeries(int seriesId, CancellationToken cancellationToken)
        {
            var series = await _databaseContext.Series.SingleOrDefaultAsync(g => g.Id == seriesId, cancellationToken);

            if (series == null)
            {
                throw new NotFoundException();
            }

            _databaseContext.Series.Remove(series);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Series>> GetSeries(CancellationToken cancellationToken)
        {
            return await _databaseContext.Series
                                         .Select(t => t.Map<Entities.Library.Series, Series>())
                                         .ToListAsync(cancellationToken);
        }

        public async Task<Series> GetSeriesById(int seriesId, CancellationToken cancellationToken)
        {
            var series = await _databaseContext.Series
                                                    .SingleOrDefaultAsync(t => t.Id == seriesId,
                                                                          cancellationToken);
            return series.Map<Entities.Library.Series, Series>();
        }
    }
}