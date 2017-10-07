using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordPageQuery : IQuery<Page<Word>>
    {
        public int DictionaryId { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}