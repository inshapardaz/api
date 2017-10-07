using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class WordMeaningByWordQuery : IQuery<IEnumerable<Meaning>>
    {
        public long WordId { get; set; }

        public string Context { get; set; }
    }
}