using System;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class AddWordMeaningCommand : Command
    {
        public long WordDetailId { get; set; }

        public Meaning Meaning { get; set; }
    }
}