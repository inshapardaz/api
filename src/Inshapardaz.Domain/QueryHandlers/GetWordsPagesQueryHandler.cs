using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordsPagesQueryHandler : QueryHandlerAsync<GetWordPageQuery, Page<Word>>
    {
        private readonly IWordRepository _wordRepository;

        public GetWordsPagesQueryHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        public override async Task<Page<Word>> ExecuteAsync(GetWordPageQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _wordRepository.GetWords(query.DictionaryId, query.PageNumber, query.PageSize, cancellationToken);
        }
    }
}