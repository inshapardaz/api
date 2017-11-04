using System;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddWordCommand : Command
    {
        public AddWordCommand(int dictionaryId, Word word)
        {
            DictionaryId = dictionaryId;
            Word = word ?? throw new ArgumentNullException(nameof(word));
        }

        public int DictionaryId { get; }

        public Word Word { get; }
    }
}
