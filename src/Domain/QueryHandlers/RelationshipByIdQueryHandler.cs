using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class RelationshipByIdQueryHandler : QueryHandler<RelationshipByIdQuery, WordRelation>
    {
        private readonly IDatabaseContext _database;

        public RelationshipByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override WordRelation Execute(RelationshipByIdQuery query)
        {
            return _database.WordRelations.SingleOrDefault(t => t.Id == query.Id);
        }
    }
}
