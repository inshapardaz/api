using System.Collections.Generic;
using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetRelationshipToWordQuery : IQuery<IEnumerable<WordRelation>>
    {
        public GetRelationshipToWordQuery(int dictionaryId, long wordId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
        }

        public int DictionaryId { get; set; }
        public long WordId { get; }
    }
}