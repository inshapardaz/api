using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordByIdQuery : IQuery<Word>
    {
        public GetWordByIdQuery(int dictionaryId, long wordId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }
    }
}