using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetTranslationsByLanguageQueryHandler : QueryHandlerAsync<GetTranslationsByLanguageQuery,
        IEnumerable<Translation>>
    {
        private readonly ITranslationRepository _translationRepository;

        public GetTranslationsByLanguageQueryHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }

        public override async Task<IEnumerable<Translation>> ExecuteAsync(GetTranslationsByLanguageQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _translationRepository.GetWordTranslationsByLanguage(query.DictionaryId, query.WordId, query.Language, cancellationToken);
        }
    }
}