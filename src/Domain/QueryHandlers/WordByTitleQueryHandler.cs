using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordByTitleQueryHandler : QueryHandler<WordByTitleQuery, Word>
    {
        private readonly IDatabaseContext _database;

        public WordByTitleQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override Word Execute(WordByTitleQuery query)
        {
            return _database.Words.SingleOrDefault(x => x.Title == query.Title);
        }
    }
}
