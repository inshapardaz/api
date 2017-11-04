using System;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordMeaningCommand : Command
    {
        public int DictionaryId { get; }

        public UpdateWordMeaningCommand(int dictionaryId, Meaning meaning)
        {
            DictionaryId = dictionaryId;
            Meaning = meaning ?? throw new ArgumentNullException(nameof(meaning));
        }

        public Meaning Meaning { get; }
    }
}