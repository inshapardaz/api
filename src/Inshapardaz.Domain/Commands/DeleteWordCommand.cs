namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordCommand : Command
    {
        public DeleteWordCommand(int dictionaryId, long wordId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }
    }
}