using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetTranslationsByLanguageQuery : IQuery<IEnumerable<Translation>>
    {
        public GetTranslationsByLanguageQuery(int dictionaryId, long wordId, Languages language)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            Language = language;
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public Languages Language { get; }
    }

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