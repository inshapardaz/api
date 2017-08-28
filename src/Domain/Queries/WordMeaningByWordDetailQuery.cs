using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class WordMeaningByWordDetailQuery : IQuery<IEnumerable<Meaning>>
    {
        public long WordDetailId { get; set; }

        public string Context { get; set; }
    }
}