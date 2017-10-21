using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddWordCommand : Command
    {
        public Word Word { get; set; }

        public int DictionaryId { get; set; }
    }
}
