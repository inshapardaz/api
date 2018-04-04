using System.Collections.Generic;
using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetTranslationsForWordsByLanguageQuery : IQuery<Dictionary<string, Translation>>
    {
        public GetTranslationsForWordsByLanguageQuery(int dictionaryId, IEnumerable<string> words, Languages language)
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