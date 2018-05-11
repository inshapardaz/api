using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordsContainingTitleQueryHandler : QueryHandlerAsync<GetWordContainingTitleQuery, Page<Word>>
    {
        private readonly IWordRepository _wordRepository;

        public GetWordsContainingTitleQueryHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        

        public override async Task<Page<Word>> ExecuteAsync(GetWordContainingTitleQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _wordRepository.GetWordsContaining(query.DictionaryId, query.SearchTerm, query.PageNumber, query.PageSize, cancellationToken);
        }
    }
}