using System.Collections.Generic;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class WordDetailsByWordQuery : IQuery<IEnumerable<WordDetail>>
    {
        public long WordId { get; set; }
    }
}