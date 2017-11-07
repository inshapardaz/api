using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Paramore.Darker;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordMeaningsByWordQueryHandler : QueryHandlerAsync<GetWordMeaningsByWordQuery, IEnumerable<Meaning>>
    {
        private readonly IDatabaseContext _database;

        public GetWordMeaningsByWordQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<Meaning>> ExecuteAsync(GetWordMeaningsByWordQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.Meaning
                    .Where(t => t.WordId == query.WordId)
                    .ToListAsync(cancellationToken);
        }
    }
}