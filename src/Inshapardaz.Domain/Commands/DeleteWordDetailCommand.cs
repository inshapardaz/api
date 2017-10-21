namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordDetailCommand : Command
    {
        public long WordDetailId { get; set; }

        public int DictionaryId { get; set; }
    }
}