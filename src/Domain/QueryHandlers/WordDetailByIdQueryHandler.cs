using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordDetailByIdQueryHandler : QueryHandler<WordDetailByIdQuery, WordDetailByIdQuery.Response>
    {
        private readonly IDatabaseContext _database;

        public WordDetailByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override WordDetailByIdQuery.Response Execute(WordDetailByIdQuery query)
        {
            var detail = _database.WordDetails
                .SingleOrDefault(w => w.Id == query.Id);
            return new WordDetailByIdQuery.Response { WordDetail = detail };
        }
    }
}
