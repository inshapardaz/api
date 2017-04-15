using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordByIdQuery : IQueryRequest<WordByIdQuery.Response>
    {
        public int Id { get; set; }

        public class Response : IQueryResponse
        {
            public Word Word { get; set; }
        }
    }
}