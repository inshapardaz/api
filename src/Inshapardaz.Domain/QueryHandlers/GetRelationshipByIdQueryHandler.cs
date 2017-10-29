using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetRelationshipByIdQueryHandler : QueryHandlerAsync<GetRelationshipByIdQuery, WordRelation>
    {
        private readonly IDatabaseContext _database;

        public GetRelationshipByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<WordRelation> ExecuteAsync(GetRelationshipByIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.WordRelation
                .Include(r => r.SourceWord)
                .Include(r => r.RelatedWord)
                .SingleOrDefaultAsync(t => t.Id == query.Id, cancellationToken);
        }
    }
}