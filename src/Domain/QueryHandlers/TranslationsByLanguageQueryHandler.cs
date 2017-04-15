using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class TranslationsByLanguageQueryHandler : QueryHandler<TranslationsByLanguageQuery, TranslationsByLanguageQuery.Response>
    {
        private readonly IDatabaseContext _database;

        public TranslationsByLanguageQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override TranslationsByLanguageQuery.Response Execute(TranslationsByLanguageQuery query)
        {
            return new TranslationsByLanguageQuery.Response
            {
                Translations = _database.Translations
                    .Where(t => t.WordDetail.WordInstanceId == query.WordId && t.Language == query.Language)
                    .ToList()
            };
        }
    }
}
