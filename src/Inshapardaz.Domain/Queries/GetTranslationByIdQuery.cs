using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetTranslationByIdQuery : IQuery<Translation>
    {
        public GetTranslationByIdQuery(int dictionaryId, long wordId, long translationId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            TranslationId = translationId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public long TranslationId { get; }
    }
}