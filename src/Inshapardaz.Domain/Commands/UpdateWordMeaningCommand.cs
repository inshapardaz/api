using System;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordMeaningCommand : Command
    {
        public int DictionaryId { get; }

        public long WordId { get; }

        public UpdateWordMeaningCommand(int dictionaryId, long wordId, Meaning meaning)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            Meaning = meaning ?? throw new ArgumentNullException(nameof(meaning));
        }

        public Meaning Meaning { get; }
    }
}