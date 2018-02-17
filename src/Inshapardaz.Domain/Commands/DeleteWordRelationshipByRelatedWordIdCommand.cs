namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordRelationshipByRelatedWordIdCommand : Command
    {
        public DeleteWordRelationshipByRelatedWordIdCommand(int dictionaryId, long wordId, long relatedWordId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            RelatedWordId = relatedWordId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public long RelatedWordId { get; }
    }
}