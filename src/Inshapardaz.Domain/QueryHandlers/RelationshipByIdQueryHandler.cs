using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class RelationshipByIdQueryHandler : QueryHandlerAsync<RelationshipByIdQuery, WordRelation>
    {
        private readonly IDatabaseContext _database;

        public RelationshipByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<WordRelation> ExecuteAsync(RelationshipByIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.WordRelation
                .Include(r => r.SourceWord)
                .Include(r => r.RelatedWord)
                .SingleOrDefaultAsync(t => t.Id == query.Id, cancellationToken);
        }
    }
}