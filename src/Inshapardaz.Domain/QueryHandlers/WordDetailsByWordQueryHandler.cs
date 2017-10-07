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
    public class WordDetailsByWordQueryHandler : QueryHandlerAsync<WordDetailsByWordQuery, IEnumerable<WordDetail>>
    {
        private readonly IDatabaseContext _database;

        public WordDetailsByWordQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<WordDetail>> ExecuteAsync(WordDetailsByWordQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.WordDetail
                .Where(w => w.WordInstanceId == query.WordId)
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }
    }
}