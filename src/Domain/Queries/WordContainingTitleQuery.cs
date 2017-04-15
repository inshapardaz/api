using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordContainingTitleQuery : IQuery<WordContainingTitleQuery.Response>
    {
        public string SearchTerm { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public class Response
        {
            public Page<Word> Page { get; set; }
        }
    }
}