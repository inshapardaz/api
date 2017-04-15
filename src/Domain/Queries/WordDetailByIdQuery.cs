using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordDetailByIdQuery : IQueryRequest<WordDetailByIdQuery.Response>
    {
        public long Id { get; set; }

        public class Response : IQueryResponse
        {
            public WordDetail WordDetail { get; set; }
        }
    }
}