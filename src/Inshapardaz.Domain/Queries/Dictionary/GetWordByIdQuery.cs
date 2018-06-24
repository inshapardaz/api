using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetWordByIdQuery : IQuery<Word>
    {
        public GetWordByIdQuery(int dictionaryId, long wordId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }
    }

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