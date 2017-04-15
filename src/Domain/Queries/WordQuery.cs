using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordQuery : IQueryRequest<WordQuery.Response>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public class Response : IQueryResponse
        {
            public Page<Word> Page { get; set; }
        }
    }
}