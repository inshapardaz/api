using System;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddDictionaryCommand : Command
    {
        public AddDictionaryCommand(Dictionary dictionary)
        {
            Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public Dictionary Dictionary { get; }
    }
}
