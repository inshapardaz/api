using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class AddWordDetailCommand : Command
    {
        public int WordId { get; set; }

        public WordDetail WordDetail { get; set; }
    }
}
