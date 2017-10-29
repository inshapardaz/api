using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddTranslationCommand : Command
    {
        public long WordId { get; set; }


        public Translation Translation { get; set; }
    }
}