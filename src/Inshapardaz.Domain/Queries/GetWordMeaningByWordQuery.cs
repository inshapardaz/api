using System.Collections.Generic;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordMeaningByWordQuery : IQuery<IEnumerable<Meaning>>
    {
        public long WordId { get; set; }

        public string Context { get; set; }
    }
}