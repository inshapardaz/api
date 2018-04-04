using System.Collections.Generic;
using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordsForTranslationsByLanguageQuery : IQuery<Dictionary<string, Word>>
    {
        public GetWordsForTranslationsByLanguageQuery(int dictionaryId, IEnumerable<string> words, Languages language)
        {
            DictionaryId = dictionaryId;
            Words = words;
            Language = language;
        }

        public int DictionaryId { get; }

        public IEnumerable<string> Words { get; }

        public Languages Language { get; }

        public bool? IsTranspiling { get; set; }
    }
}