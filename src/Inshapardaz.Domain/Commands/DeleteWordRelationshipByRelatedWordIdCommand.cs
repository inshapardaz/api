namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordRelationshipByRelatedWordIdCommand : Command
    {
        public DeleteWordRelationshipByRelatedWordIdCommand(int dictionaryId, long relatedWordId)
        {
            DictionaryId = dictionaryId;
            RelatedWordId = relatedWordId;
        }

        public int DictionaryId { get; }

        public long RelatedWordId { get; }
    }
}