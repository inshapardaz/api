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
    public class WordMeaningByWordQueryHandler : AsyncQueryHandler<WordMeaningByWordQuery, IEnumerable<Meaning>>
    {
        private readonly IDatabaseContext _database;

        public WordMeaningByWordQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<Meaning>> ExecuteAsync(WordMeaningByWordQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(query.Context))
            {
                return await _database.Meanings
                           .Where(t => t.WordDetail.WordInstanceId == query.WordId)
                           .ToListAsync();
            }
            else
            {
                return await _database.Meanings
                    .Where(t => t.WordDetail.WordInstanceId == query.WordId && t.Context == query.Context)
                    .ToListAsync();
            }
        }
    }
}