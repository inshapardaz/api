using System;

namespace Inshapardaz.Domain.Commands
{
    public class DeleteDictionaryCommand : Command
    {
        public int DictionaryId { get; set; }
    }
}
