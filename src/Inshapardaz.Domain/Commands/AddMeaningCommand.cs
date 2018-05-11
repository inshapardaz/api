using System;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddMeaningCommand : Command
    {
        public AddMeaningCommand(int dictionaryId, long wordId, Meaning meaning)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            Meaning = meaning?? throw new ArgumentNullException(nameof(meaning));
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public Meaning Meaning { get; }

        public Meaning Result { get; set; }
    }
}