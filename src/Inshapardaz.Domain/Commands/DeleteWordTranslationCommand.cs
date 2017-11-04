namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordTranslationCommand : Command
    {
        public DeleteWordTranslationCommand(int dictionaryId, long translationId)
        {
            DictionaryId = dictionaryId;
            TranslationId = translationId;
        }

        public int DictionaryId { get; }

        public long TranslationId { get; }
    }
}