using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class AddDictionaryCommand : Command
    {
        public Dictionary Dictionary { get; set; }
    }
}
