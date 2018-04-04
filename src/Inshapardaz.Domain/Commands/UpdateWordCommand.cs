using System;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordCommand : Command
    {
        public int DictionaryId { get; }

        public UpdateWordCommand(int dictionaryId, Word word)
        {
            DictionaryId = dictionaryId;
            Word = word ?? throw new ArgumentNullException(nameof(word));
        }

        public Word Word { get; }
    }
}
