using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordTranslationCommand : Command
    {
        public Translation Translation { get; set; }
    }
}
