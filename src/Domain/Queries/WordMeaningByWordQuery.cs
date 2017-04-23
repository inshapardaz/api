using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordMeaningByWordQuery : IQuery<IEnumerable<Meaning>>
    {
        public int WordId { get; set; }

        public string Context { get; set; }
    }
}