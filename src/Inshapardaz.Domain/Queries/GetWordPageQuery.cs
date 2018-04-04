using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordPageQuery : IQuery<Page<Word>>
    {
        public GetWordPageQuery(int dictionaryId, int pageNumber, int pageSize)
        {
            DictionaryId = dictionaryId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int DictionaryId { get; }

        public int PageNumber { get; }

        public int PageSize { get; }
    }
}