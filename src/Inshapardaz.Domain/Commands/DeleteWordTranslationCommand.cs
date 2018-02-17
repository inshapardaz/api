namespace Inshapardaz.Domain.Commands
{
    public class DeleteWordTranslationCommand : Command
    {
        public DeleteWordTranslationCommand(int dictionaryId, long wordId, long translationId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            TranslationId = translationId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public long TranslationId { get; }
    }
}