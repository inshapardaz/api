using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordByIdQueryHandler : QueryHandler<WordByIdQuery, Word>
    {
        private readonly IDatabaseContext _database;

        public WordByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override Word Execute(WordByIdQuery query)
        {
            return _database.Words.SingleOrDefault(w => w.Id == query.Id && (w.Dictionary.IsPublic || w.Dictionary.UserId == query.UserId));
        }
    }
}
