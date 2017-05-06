using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordMeaningCommand : Command
    {
        public long MeaningId { get; set; }
    }
}