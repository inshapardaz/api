using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using System.Linq;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordMeaningByIdQueryHandler : QueryHandler<WordMeaningByIdQuery, Meaning>
    {
        private readonly IDatabaseContext _database;

        public WordMeaningByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override Meaning Execute(WordMeaningByIdQuery query)
        {
            return _database.Meanings.SingleOrDefault(t => t.Id == query.Id);
        }
    }
}