using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class  WordDetailsByWordQuery : IQuery<IEnumerable<WordDetail>>
    {
        public int WordId { get; set; }
    }
}