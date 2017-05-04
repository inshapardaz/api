using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class RelationshipByWordIdQueryHandler : AsyncQueryHandler<RelationshipByWordIdQuery, IEnumerable<WordRelation>>
    {
        private readonly IDatabaseContext _database;

        public RelationshipByWordIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<WordRelation>> ExecuteAsync(RelationshipByWordIdQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.WordRelations
                    .Include(r => r.RelatedWord)
                    .Include(r => r.SourceWord)
                    .Where(t => t.SourceWordId == query.WordId)
                    .ToListAsync();
        }
    }
}