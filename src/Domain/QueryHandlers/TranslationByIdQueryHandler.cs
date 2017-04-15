using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class TranslationByIdQueryHandler : QueryHandler<TranslationByIdQuery, TranslationByIdQuery.Response>
    {
        private readonly IDatabaseContext _database;

        public TranslationByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override TranslationByIdQuery.Response Execute(TranslationByIdQuery args)
        {
            return new TranslationByIdQuery.Response
            {
                Translation = _database.Translations.SingleOrDefault(t => t.Id == args.Id)
            };
        }
    }
}
