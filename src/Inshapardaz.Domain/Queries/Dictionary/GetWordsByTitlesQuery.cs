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
    public class GetWordsByTitlesQuery : IQuery<IEnumerable<Word>>
    {
        public GetWordsByTitlesQuery(int dictionaryId, IEnumerable<string> titles)
        {
            DictionaryId = dictionaryId;
            Titles = titles;
        }

        public int DictionaryId { get; }
        public IEnumerable<string> Titles { get; }
    }

    public class GetWordsByTitlesQueryHandler : QueryHandlerAsync<GetWordsByTitlesQuery, IEnumerable<Word>>
    {
        private readonly IWordRepository _wordRepository;

        public GetWordsByTitlesQueryHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }


        public override async Task<IEnumerable<Word>> ExecuteAsync(GetWordsByTitlesQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _wordRepository.GetWordsByTitles(query.DictionaryId, query.Titles, cancellationToken);
        }
    }
}