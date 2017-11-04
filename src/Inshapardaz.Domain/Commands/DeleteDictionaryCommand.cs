using System;

namespace Inshapardaz.Domain.Commands
{
    public class DeleteDictionaryCommand : Command
    {
        public DeleteDictionaryCommand(int dictionaryId)
        {
            DictionaryId = dictionaryId;
        }

        public int DictionaryId { get; }
    }
}
