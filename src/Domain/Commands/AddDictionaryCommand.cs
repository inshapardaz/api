using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class AddDictionaryCommand : Command
    {
        public string UserId { get; set; }
        public Dictionary Dictionary { get; set; }
    }
}
