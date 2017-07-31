using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class RelationshipByIdQueryHandler : AsyncQueryHandler<RelationshipByIdQuery, WordRelation>
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