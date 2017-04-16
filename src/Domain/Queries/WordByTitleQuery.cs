using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordByTitleQuery : IQuery<WordByTitleQuery.Response>
    {
        public string Title { get; set; }
        public string UserId { get; set; }

        public class Response
        {
            public Word Word { get; set; }
        }
    }
}