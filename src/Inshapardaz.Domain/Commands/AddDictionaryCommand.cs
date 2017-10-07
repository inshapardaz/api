using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddDictionaryCommand : Command
    {
        public Dictionary Dictionary { get; set; }
    }
}
