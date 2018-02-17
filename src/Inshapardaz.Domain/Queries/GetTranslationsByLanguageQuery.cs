using System.Collections.Generic;
using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
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
}