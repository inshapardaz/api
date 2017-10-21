using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordDetailCommand : Command
    {
        public long WordId { get; set; }

        public WordDetail WordDetail { get; set; }

        public int DictionaryId { get; set; }
    }
}
