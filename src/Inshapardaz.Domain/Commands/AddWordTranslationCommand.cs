using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddWordTranslationCommand : Command
    {
        public long WordId { get; set; }

        public long WordDetailId { get; set; }

        public Translation Translation { get; set; }
    }
}