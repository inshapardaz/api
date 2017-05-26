using Darker;

using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordStartingWithQuery : IQuery<Page<Word>>
    {
        public string Title { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
        public int DictionaryId { get; set; }
    }
}