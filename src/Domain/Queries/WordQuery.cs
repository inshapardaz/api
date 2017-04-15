using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordQuery : IQuery<WordQuery.Response>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public class Response
        {
            public Page<Word> Page { get; set; }
        }
    }
}