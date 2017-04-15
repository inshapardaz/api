using System.Linq;

using Darker;

using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class TranslationsByWordIdQueryHandler : QueryHandler<TranslationsByWordIdQuery, TranslationsByWordIdQuery.Response>
    {
        private readonly IDatabase _database;

        public TranslationsByWordIdQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public override TranslationsByWordIdQuery.Response Execute(TranslationsByWordIdQuery query)
        {
            return new TranslationsByWordIdQuery.Response
            {
                Translations = _database.Translations
                    .Where(t => t.WordDetail.WordInstanceId == query.WordId)
                    .ToList()
            };
        }
    }
}
