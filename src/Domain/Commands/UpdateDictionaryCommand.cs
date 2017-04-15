using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateDictionaryCommand : Command
    {
        public string UserId { get; set; }
        public Dictionary Dictionary { get; set; }
    }
}
