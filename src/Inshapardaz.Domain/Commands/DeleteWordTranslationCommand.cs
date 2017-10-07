namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordTranslationCommand : Command
    {
        public long TranslationId { get; set; }
    }
}