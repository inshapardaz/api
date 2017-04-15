using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordByIdQueryHandler : QueryHandler<WordByIdQuery, WordByIdQuery.Response>
    {
        private readonly IDatabaseContext _database;

        public WordByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override WordByIdQuery.Response Execute(WordByIdQuery query)
        {
            return new WordByIdQuery.Response {Word = _database.Words.SingleOrDefault(w => w.Id == query.Id)};
        }
    }
}
