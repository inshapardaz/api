using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordMeaningByIdQuery : IQuery<Meaning>
    {
        public GetWordMeaningByIdQuery(int dictionaryId, long wordId, long meaningId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            MeaningId = meaningId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public long MeaningId { get; }
    }
}