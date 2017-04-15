using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class AddWordCommand : Command
    {
        public Word Word { get; set; }
    }
}
