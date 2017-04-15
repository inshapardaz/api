using System.Collections.Generic;
using System.Linq;
using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordMeaningByWordQueryHandler : QueryHandler<WordMeaningByWordQuery, WordMeaningByWordQuery.Response>
    {
        private readonly IDatabaseContext _database;

        public WordMeaningByWordQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override WordMeaningByWordQuery.Response Execute(WordMeaningByWordQuery args)
        {
            IEnumerable<Meaning> meanings;
            if (string.IsNullOrWhiteSpace(args.Context))
            {
                meanings =_database.Meanings
                           .Where(t => t.WordDetail.WordInstanceId == args.WordId)
                           .ToList();
            }
            else
            {
                meanings = _database.Meanings
                    .Where(t => t.WordDetail.WordInstanceId == args.WordId && t.Context == args.Context)
                    .ToList();
            }

            return new WordMeaningByWordQuery.Response {Meanings = meanings };
        }
    }
}
