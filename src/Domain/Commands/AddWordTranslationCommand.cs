using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class AddWordTranslationCommand : Command
    {
        public int WordId { get; set; }

        public int WordDetailId  { get; set; }

        public Translation Translation { get; set; }
    }
}
