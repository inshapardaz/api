using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
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