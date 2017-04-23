using System.Collections.Generic;
using System.Linq;
using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordMeaningByWordQueryHandler : QueryHandler<WordMeaningByWordQuery, IEnumerable<Meaning>>
    {
        private readonly IDatabaseContext _database;

        public WordMeaningByWordQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override IEnumerable<Meaning> Execute(WordMeaningByWordQuery args)
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

            return meanings;
        }
    }
}
