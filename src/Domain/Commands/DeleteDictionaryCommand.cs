using System;

namespace Inshapardaz.Domain.Commands
{
    public class DeleteDictionaryCommand : Command
    {
        public Guid UserId { get; set; }

        public int DictionaryId { get; set; }
    }
}
