using System.Threading;
using System.Threading.Tasks;
using Paramore.Darker;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByWordDetailIdQueryHandler : QueryHandlerAsync<DictionaryByWordDetailIdQuery, Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByWordDetailIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Dictionary> ExecuteAsync(DictionaryByWordDetailIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var word = await _database.WordDetail
                .Include(wd => wd.WordInstance)
                .ThenInclude(w => w.Dictionary).SingleOrDefaultAsync(wd => wd.Id == query.WordDetailId,
                cancellationToken);
            return word?.WordInstance.Dictionary;
        }
    }
}