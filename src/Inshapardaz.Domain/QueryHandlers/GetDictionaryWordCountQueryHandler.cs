using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryWordCountQueryHandler : QueryHandlerAsync<GetDictionaryWordCountQuery, int>
    {
        private readonly IWordRepository _wordRepository;

        public GetDictionaryWordCountQueryHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        public override async Task<int> ExecuteAsync(GetDictionaryWordCountQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _wordRepository.GetWordCountByDictionary(query.DictionaryId, cancellationToken);
        }
    }
}