using System;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordTranslationCommand : Command
    {
        public int DictionaryId { get; }

        public UpdateWordTranslationCommand(int dictionaryId, long wordId, Translation translation)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            Translation = translation ?? throw new ArgumentNullException(nameof(translation));
        }

        public long WordId { get; }

        public Translation Translation { get; }
    }
}
