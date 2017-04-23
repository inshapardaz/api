using System.Linq;
using Darker;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class TranslationByIdQueryHandler : QueryHandler<TranslationByIdQuery, Translation>
    {
        private readonly IDatabaseContext _database;

        public TranslationByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override Translation Execute(TranslationByIdQuery args)
        {
            return _database.Translations.SingleOrDefault(t => t.Id == args.Id);
        }
    }
}
