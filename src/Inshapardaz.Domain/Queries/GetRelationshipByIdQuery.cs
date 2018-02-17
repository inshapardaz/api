using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetRelationshipByIdQuery : IQuery<WordRelation>
    {
        public GetRelationshipByIdQuery(int dictionaryId, long wordId, long relationRelationshipId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            RelationshipId = relationRelationshipId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public long RelationshipId { get; }
    }
}