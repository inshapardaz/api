using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordByTitleQuery : IQueryRequest<WordByTitleQuery.Response>
    {
        public string Title { get; set; }

        public class Response : IQueryResponse
        {
            public Word Word { get; set; }
        }
    }
}