using Darker;

using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordStartingWithQuery : IQuery<WordStartingWithQuery.Response>
    {
        public string Title { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public class Response
        {
            public Page<Word> Page { get; set; } 
        }
    }
}