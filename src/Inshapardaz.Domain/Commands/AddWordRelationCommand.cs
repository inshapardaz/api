using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddWordRelationCommand : Command
    {
        public AddWordRelationCommand(int dictionaryId, long sourceWordId, long relatedWordId, RelationType relationType)
        {
            DictionaryId = dictionaryId;
            SourceWordId = sourceWordId;
            RelatedWordId = relatedWordId;
            RelationType = relationType;
        }

        public int DictionaryId { get; }

        public long SourceWordId { get; }
        public long RelatedWordId { get; }
        public RelationType RelationType { get; }

        public long Result { get; set; }
    }
}