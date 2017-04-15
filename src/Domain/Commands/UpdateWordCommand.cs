using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordCommand : Command
    {
        public Word Word { get; set; }
    }
}
