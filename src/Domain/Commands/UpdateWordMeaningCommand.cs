using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordMeaningCommand : Command
    {
        public Meaning Meaning { get; set; }
    }
}