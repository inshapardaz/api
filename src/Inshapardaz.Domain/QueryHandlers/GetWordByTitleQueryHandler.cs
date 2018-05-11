using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordByTitleQueryHandler : QueryHandlerAsync<GetWordByTitleQuery, Word>
    {
        private readonly IWordRepository _wordRepository;

        public GetWordByTitleQueryHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        public override async Task<Word> ExecuteAsync(GetWordByTitleQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _wordRepository.GetWordByTitle(query.DictionaryId, query.Title, cancellationToken);
        }
    }
}