using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class RelationshipByWordIdQueryHandler : QueryHandler<RelationshipByWordIdQuery, RelationshipByWordIdQuery.Response>
    {
        private readonly IDatabase _database;

        public RelationshipByWordIdQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public override RelationshipByWordIdQuery.Response Execute(RelationshipByWordIdQuery query)
        {
            return new RelationshipByWordIdQuery.Response
            {
                Relations = _database.WordRelations
                    .Include(r => r.RelatedWord)
                    .Where(t => t.SourceWordId == query.WordId)
                    .ToList()
            };
        }
    }
}