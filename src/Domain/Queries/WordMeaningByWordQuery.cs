using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordMeaningByWordQuery : IQuery<WordMeaningByWordQuery.Response>
    {
        public int WordId { get; set; }

        public string Context { get; set; }

        public class Response
        {
            public IEnumerable<Meaning> Meanings { get; set; }
        }
    }
}