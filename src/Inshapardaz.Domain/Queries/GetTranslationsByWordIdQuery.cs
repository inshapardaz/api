using System.Collections.Generic;
using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetTranslationsByWordIdQuery : IQuery<IEnumerable<Translation>>
    {
        public GetTranslationsByWordIdQuery(int dictionaryId, long wordId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }
    }
}