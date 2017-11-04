namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordRelationshipCommand : Command
    {
        public DeleteWordRelationshipCommand(int dictionaryId, long relationshipId)
        {
            DictionaryId = dictionaryId;
            RelationshipId = relationshipId;
        }

        public int DictionaryId { get; }

        public long RelationshipId { get; }
    }
}