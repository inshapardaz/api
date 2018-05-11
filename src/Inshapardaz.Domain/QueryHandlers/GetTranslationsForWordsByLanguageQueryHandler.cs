using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetTranslationsForWordsByLanguageQueryHandler : QueryHandlerAsync<GetTranslationsForWordsByLanguageQuery, Dictionary<string, Translation>>
    {
        private readonly IWordRepository _wordRepository;
        private readonly ITranslationRepository _translationRepository;

        public GetTranslationsForWordsByLanguageQueryHandler(IWordRepository wordRepository, ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
            _wordRepository = wordRepository;
        }
        
        public override async Task<Dictionary<string, Translation>> ExecuteAsync(GetTranslationsForWordsByLanguageQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            var words = await _wordRepository.GetWordsByTitles(query.DictionaryId, query.Words, cancellationToken);

            var result = new Dictionary<string, Translation>();
            foreach (var word in words)
            {
                var translations = await _translationRepository.GetWordTranslationsByLanguage(query.DictionaryId, word.Id, query.Language, cancellationToken);
                if (result.ContainsKey(word.Title))
                {
                    // IMPORVE : We have mulitple translations for this word. 
                    continue;
                }

                Translation translation;
                if (query.IsTranspiling.HasValue)
                {
                    translation = translations.FirstOrDefault(t => t.IsTrasnpiling == query.IsTranspiling);
                }
                else
                {
                    translation = translations.FirstOrDefault();
                }

                if (translation != null)
                {
                    result.Add(word.Title, translation);
                }
            }
            return result;

        }
    }
}