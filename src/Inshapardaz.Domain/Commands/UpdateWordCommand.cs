using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordCommand : Command
    {
        public Word Word { get; set; }
    }
}
