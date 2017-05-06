using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByTranslationIdQueryHandler : AsyncQueryHandler<DictionaryByTranslationIdQuery, Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByTranslationIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public async override Task<Dictionary> ExecuteAsync(DictionaryByTranslationIdQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var meaning = await _database.Translations.SingleOrDefaultAsync(m => m.Id == query.TranslationId);
            return meaning?.WordDetail.WordInstance.Dictionary;
        }
    }
}