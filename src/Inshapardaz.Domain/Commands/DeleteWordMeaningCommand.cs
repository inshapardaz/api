namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordMeaningCommand : Command
    {
        public DeleteWordMeaningCommand(int dictionaryId, long wordId, long meaningId)
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