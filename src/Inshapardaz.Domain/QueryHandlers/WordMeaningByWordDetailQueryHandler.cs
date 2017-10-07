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
    public class WordMeaningByWordDetailQueryHandler : QueryHandlerAsync<WordMeaningByWordDetailQuery, IEnumerable<Meaning>>
    {
        private readonly IDatabaseContext _database;

        public WordMeaningByWordDetailQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<Meaning>> ExecuteAsync(WordMeaningByWordDetailQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(query.Context))
            {
                return await _database.Meaning
                    .Where(t => t.WordDetail.Id == query.WordDetailId)
                    .ToListAsync(cancellationToken);
            }

            return await _database.Meaning
                .Where(t => t.WordDetail.WordInstanceId == query.WordDetailId && t.Context == query.Context)
                .ToListAsync(cancellationToken);
        }
    }
}