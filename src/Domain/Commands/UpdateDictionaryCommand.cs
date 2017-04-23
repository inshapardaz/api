using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateDictionaryCommand : Command
    {
        public Dictionary Dictionary { get; set; }
    }
}
