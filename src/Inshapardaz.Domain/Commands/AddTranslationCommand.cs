using System;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddTranslationCommand : Command
    {
        public AddTranslationCommand(int dictionaryId, long wordId, Translation translation)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            Translation = translation ?? throw new ArgumentNullException(nameof(translation));
        }

        public int DictionaryId { get; }

        public long WordId { get; }


        public Translation Translation { get;  }
    }
}