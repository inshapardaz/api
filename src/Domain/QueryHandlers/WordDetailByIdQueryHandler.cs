using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordDetailByIdQueryHandler : QueryHandler<WordDetailByIdQuery, WordDetail>
    {
        private readonly IDatabaseContext _database;

        public WordDetailByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override WordDetail Execute(WordDetailByIdQuery query)
        {
            return _database.WordDetails
                .SingleOrDefault(w => w.Id == query.Id);
        }
    }
}
