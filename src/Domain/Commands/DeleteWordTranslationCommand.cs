namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordTranslationCommand : Command
    {
        public int TranslationId { get; set; }
    }
}