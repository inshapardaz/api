using System;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddTranslationCommand : Command
    {
        public AddTranslationCommand(int dictioanryId, long wordId, Translation translation)
        {
            DictioanryId = dictioanryId;
            WordId = wordId;
            Translation = translation ?? throw new ArgumentNullException(nameof(translation));
        }

        public int DictioanryId { get; }

        public long WordId { get; }


        public Translation Translation { get;  }
    }
}