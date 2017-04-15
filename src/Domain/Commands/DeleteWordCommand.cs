using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordCommand : Command
    {
        public Word Word { get; set; }
    }
}
