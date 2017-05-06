using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordCommand : Command
    {
        public long WordId { get; set; }
    }
}