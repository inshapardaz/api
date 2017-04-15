using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordMeaningByWordQuery : IQueryRequest<WordMeaningByWordQuery.Response>
    {
        public int WordId { get; set; }

        public string Context { get; set; }

        public class Response : IQueryResponse
        {
            public IEnumerable<Meaning> Meanings { get; set; }
        }
    }
}