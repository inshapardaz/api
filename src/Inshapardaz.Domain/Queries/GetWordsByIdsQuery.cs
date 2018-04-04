using System.Collections.Generic;
using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordsByIdsQuery : IQuery<Page<Word>>
    {
        public GetWordsByIdsQuery(int dictionaryId, IEnumerable<long> ids)
        {
            DictionaryId = dictionaryId;
            IDs = ids;
        }

        public int DictionaryId { get; }

        public IEnumerable<long> IDs { get; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}