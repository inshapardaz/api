using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordDetailsByWordQuery : IQuery<IEnumerable<WordDetail>>
    {
        public long WordId { get; set; }
    }
}