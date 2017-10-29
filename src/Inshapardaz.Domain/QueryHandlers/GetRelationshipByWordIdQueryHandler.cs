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
    public class GetRelationshipByWordIdQueryHandler : QueryHandlerAsync<GetRelationshipByWordIdQuery,
        IEnumerable<WordRelation>>
    {
        private readonly IDatabaseContext _database;

        public GetRelationshipByWordIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<WordRelation>> ExecuteAsync(GetRelationshipByWordIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.WordRelation
                .Include(r => r.RelatedWord)
                .Include(r => r.SourceWord)
                .Where(t => t.SourceWordId == query.WordId)
                .ToListAsync(cancellationToken);
        }
    }
}