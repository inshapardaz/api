using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordMeaningByIdQuery : IQueryRequest<WordMeaningByIdQuery.Response>
    {
        public int Id { get; set; }

        public class Response : IQueryResponse
        {
            public Meaning Meaning { get; set; }
        }
    }
}