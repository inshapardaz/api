using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using System.Linq;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordMeaningByIdQueryHandler : QueryHandler<WordMeaningByIdQuery, WordMeaningByIdQuery.Response>
    {
        private readonly IDatabase _database;

        public WordMeaningByIdQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public override WordMeaningByIdQuery.Response Execute(WordMeaningByIdQuery query)
        {
            return new WordMeaningByIdQuery.Response
            {
                Meaning = _database.Meanings.SingleOrDefault(t => t.Id == query.Id)
            };
        }
    }
}