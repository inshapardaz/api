using System;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddTranslationCommand : Command
    {
        public AddTranslationCommand(long wordId, Translation translation)
        {
            WordId = wordId;
            Translation = translation ?? throw new ArgumentNullException(nameof(translation));
        }


        public long WordId { get; }


        public Translation Translation { get;  }
    }
}