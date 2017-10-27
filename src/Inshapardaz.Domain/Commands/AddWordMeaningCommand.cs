using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddWordMeaningCommand : Command
    {
        public long WordDetailId { get; set; }

        public Meaning Meaning { get; set; }

    }
}