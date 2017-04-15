using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordMeaningCommand : Command
    {
        public Meaning Meaning { get; set; }
    }
}