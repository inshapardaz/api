using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordContainingTitleQuery : IQueryRequest<WordContainingTitleQuery.Response>
    {
        public string SearchTerm { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public class Response : IQueryResponse
        {
            public Page<Word> Page { get; set; }
        }
    }
}