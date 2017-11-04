using System;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordTranslationCommand : Command
    {
        public int DictionaryId { get; }

        public UpdateWordTranslationCommand(int dictionaryId, Translation translation)
        {
            DictionaryId = dictionaryId;
            Translation = translation ?? throw new ArgumentNullException(nameof(translation));
        }

        public Translation Translation { get; }
    }
}
