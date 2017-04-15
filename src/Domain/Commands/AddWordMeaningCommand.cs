using System;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class AddWordMeaningCommand  : Command
    {
        public int WordDetailId { get; set; }

        public Meaning Meaning { get; set; }
    }
}
