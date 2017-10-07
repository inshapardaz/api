using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordMeaningCommand : Command
    {
        public Meaning Meaning { get; set; }
    }
}