using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordStartingWithQuery : IQuery<Page<Word>>
    {
        public string Title { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
        public int DictionaryId { get; set; }
    }
}