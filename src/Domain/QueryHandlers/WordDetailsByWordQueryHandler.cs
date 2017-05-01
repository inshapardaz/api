using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordDetailsByWordQueryHandler : AsyncQueryHandler<WordDetailsByWordQuery, IEnumerable<WordDetail>>
    {
        private readonly IDatabaseContext _database;

        public WordDetailsByWordQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }
        
        public async override Task<IEnumerable<WordDetail>> ExecuteAsync(WordDetailsByWordQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<WordDetail> result;

            result = await _database.WordDetails
                              .Where(w => w.WordInstanceId == query.WordId)
                              .OrderBy(x => x.Id)
                              .ToListAsync();
            return result;
        }
    }
}
