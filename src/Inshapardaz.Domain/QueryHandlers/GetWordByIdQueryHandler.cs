using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordByIdQueryHandler : QueryHandlerAsync<GetWordByIdQuery, Word>
    {
        private readonly IWordRepository _wordRepository;

        public GetWordByIdQueryHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        public override async Task<Word> ExecuteAsync(GetWordByIdQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _wordRepository.GetWordById(query.DictionaryId, query.WordId, cancellationToken);
        }
    }
}