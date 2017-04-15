using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class RelationshipByIdQueryHandler : QueryHandler<RelationshipByIdQuery, RelationshipByIdQuery.Response>
    {
        private readonly IDatabaseContext _database;

        public RelationshipByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override RelationshipByIdQuery.Response Execute(RelationshipByIdQuery query)
        {
            return new RelationshipByIdQuery.Response
            {
                Relation = _database.WordRelations.SingleOrDefault(t => t.Id == query.Id)
            };
        }
    }
}
