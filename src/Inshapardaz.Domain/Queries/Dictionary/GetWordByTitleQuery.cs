using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetWordByTitleQuery : IQuery<Word>
    {
        public GetWordByTitleQuery(int dictionaryId, string title)
        {
            DictionaryId = dictionaryId;
            Title = title;
        }

        public int DictionaryId { get; }
        public string Title { get; }
    }

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