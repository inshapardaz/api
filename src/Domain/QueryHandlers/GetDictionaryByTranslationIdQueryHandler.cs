using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class
        GetDictionaryByTranslationIdQueryHandler : AsyncQueryHandler<DictionaryByTranslationIdQuery, Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByTranslationIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Dictionary> ExecuteAsync(DictionaryByTranslationIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var meaning =
                await _database.Translation.SingleOrDefaultAsync(m => m.Id == query.TranslationId, cancellationToken);
            return meaning?.WordDetail.WordInstance.Dictionary;
        }
    }
}