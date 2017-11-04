namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordMeaningCommand : Command
    {
        public DeleteWordMeaningCommand(int dictionaryId, long meaningId)
        {
            DictionaryId = dictionaryId;
            MeaningId = meaningId;
        }

        public int DictionaryId { get; }

        public long MeaningId { get; }
    }
}