using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordByIdQuery : IQuery<WordByIdQuery.Response>
    {
        public int Id { get; set; }

        public class Response
        {
            public Word Word { get; set; }
        }
    }
}