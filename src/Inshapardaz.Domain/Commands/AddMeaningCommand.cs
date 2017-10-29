using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddMeaningCommand : Command
    {
        public long WordId { get; set; }

        public Meaning Meaning { get; set; }

    }
}