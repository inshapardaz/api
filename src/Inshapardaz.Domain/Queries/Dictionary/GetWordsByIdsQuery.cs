using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
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

    public class GetWordsByIdsQueryHandler : QueryHandlerAsync<GetWordsByIdsQuery, Page<Word>>
    {
        private readonly IWordRepository _wordRepository;

        public GetWordsByIdsQueryHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }


        public override async Task<Page<Word>> ExecuteAsync(GetWordsByIdsQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _wordRepository.GetWordsById(query.DictionaryId, query.IDs, query.PageNumber, query.PageSize, cancellationToken);
        }
    }
}