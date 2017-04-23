using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;
using Inshapardaz.Domain.Model;
using System.Collections.Generic;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class RelationshipByWordIdQueryHandler : QueryHandler<RelationshipByWordIdQuery, IEnumerable<WordRelation>>
    {
        private readonly IDatabaseContext _database;

        public RelationshipByWordIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override IEnumerable<WordRelation> Execute(RelationshipByWordIdQuery query)
        {
            return _database.WordRelations
                    .Include(r => r.RelatedWord)
                    .Where(t => t.SourceWordId == query.WordId)
                    .ToList();
        }
    }
}