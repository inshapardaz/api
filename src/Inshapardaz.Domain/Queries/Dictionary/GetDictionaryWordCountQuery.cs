using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetDictionaryWordCountQuery : IQuery<int>
    {
        public int DictionaryId { get; set; }
    }

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