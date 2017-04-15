using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordDetailByIdQuery : IQuery<WordDetailByIdQuery.Response>
    {
        public long Id { get; set; }

        public class Response
        {
            public WordDetail WordDetail { get; set; }
        }
    }
}