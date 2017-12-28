using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetRelationshipToWordQueryHandler : QueryHandlerAsync<GetRelationshipToWordQuery,
        IEnumerable<WordRelation>>
    {
        private readonly IDatabaseContext _database;

        public GetRelationshipToWordQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<WordRelation>> ExecuteAsync(GetRelationshipToWordQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.WordRelation
                .Include(r => r.RelatedWord)
                .Include(r => r.SourceWord)
                .Where(t => t.RelatedWordId == query.WordId)
                .ToListAsync(cancellationToken);
        }
    }
}