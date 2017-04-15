using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class  WordDetailsByWordQuery : IQueryRequest<WordDetailsByWordQuery.Response>
    {
        public int WordId { get; set; }

        public bool IncludeDetails { get; set; }

        public class Response : IQueryResponse
        {
            public IEnumerable<WordDetail> WordDetail { get; set; }
        }
    }
}