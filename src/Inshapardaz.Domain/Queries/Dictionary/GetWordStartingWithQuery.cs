using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetWordStartingWithQuery : IQuery<Page<Word>>
    {
        public string Title { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
        public int DictionaryId { get; set; }
    }

    public class GetWordStartingWithQueryHandler : QueryHandlerAsync<GetWordStartingWithQuery, Page<Word>>
    {
        private readonly IWordRepository _wordRepository;

        public GetWordStartingWithQueryHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        public override async Task<Page<Word>> ExecuteAsync(GetWordStartingWithQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _wordRepository.GetWords(query.DictionaryId, query.PageNumber, query.PageSize, cancellationToken);
        }
    }
}