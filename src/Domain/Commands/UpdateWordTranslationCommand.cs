using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordTranslationCommand : Command
    {
        public Translation Translation { get; set; }
    }
}
