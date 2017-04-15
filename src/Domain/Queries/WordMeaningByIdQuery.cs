using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordMeaningByIdQuery : IQuery<WordMeaningByIdQuery.Response>
    {
        public int Id { get; set; }

        public class Response
        {
            public Meaning Meaning { get; set; }
        }
    }
}