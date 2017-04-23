using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;
using System.Collections.Generic;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class TranslationsByLanguageQueryHandler : QueryHandler<TranslationsByLanguageQuery, IEnumerable<Translation>>
    {
        private readonly IDatabaseContext _database;

        public TranslationsByLanguageQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override IEnumerable<Translation> Execute(TranslationsByLanguageQuery query)
        {
            return _database.Translations
                    .Where(t => t.WordDetail.WordInstanceId == query.WordId && t.Language == query.Language)
                    .ToList();
        }
    }
}
