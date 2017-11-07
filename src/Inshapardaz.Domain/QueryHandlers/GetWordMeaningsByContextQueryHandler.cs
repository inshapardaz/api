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
    public class GetWordMeaningsByContextQueryHandler : QueryHandlerAsync<GetWordMeaningsByContextQuery, IEnumerable<Meaning>>
    {
        private readonly IDatabaseContext _database;

        public GetWordMeaningsByContextQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<Meaning>> ExecuteAsync(GetWordMeaningsByContextQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(query.Context))
            {
                return await _database.Meaning
                    .Where(t => t.WordId == query.WordId)
                    .ToListAsync(cancellationToken);
            }

            return await _database.Meaning
                .Where(t => t.WordId == query.WordId && t.Context == query.Context)
                .ToListAsync(cancellationToken);
        }
    }
}