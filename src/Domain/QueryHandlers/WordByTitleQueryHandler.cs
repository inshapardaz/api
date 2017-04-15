using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordByTitleQueryHandler : QueryHandler<WordByTitleQuery, WordByTitleQuery.Response>
    {
        private readonly IDatabase _database;

        public WordByTitleQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public override WordByTitleQuery.Response Execute(WordByTitleQuery query)
        {
            return new WordByTitleQuery.Response
            {
                Word = _database.Words.SingleOrDefault(x => x.Title == query.Title)
            };
        }
    }
}
