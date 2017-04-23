using System.Linq;

using Darker;

using Inshapardaz.Domain.Queries;
using System.Collections.Generic;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class TranslationsByWordIdQueryHandler : QueryHandler<TranslationsByWordIdQuery, IEnumerable<Translation>>
    {
        private readonly IDatabaseContext _database;

        public TranslationsByWordIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override IEnumerable<Translation> Execute(TranslationsByWordIdQuery query)
        {
            return _database.Translations
                    .Where(t => t.WordDetail.WordInstanceId == query.WordId)
                    .ToList();
        }
    }
}
