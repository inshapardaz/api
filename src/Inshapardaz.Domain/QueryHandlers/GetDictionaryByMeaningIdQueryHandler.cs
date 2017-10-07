using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByMeaningIdQueryHandler : AsyncQueryHandler<DictionaryByMeaningIdQuery, Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByMeaningIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Dictionary> ExecuteAsync(DictionaryByMeaningIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var meaning = await _database.Meaning
                                    .Include(m => m.WordDetail)
                                    .ThenInclude(wd => wd.WordInstance)
                                    .ThenInclude(w => w.Dictionary)
                                    .SingleOrDefaultAsync(m => m.Id == query.MeaningId, cancellationToken);
            return meaning?.WordDetail.WordInstance.Dictionary;
        }
    }
}