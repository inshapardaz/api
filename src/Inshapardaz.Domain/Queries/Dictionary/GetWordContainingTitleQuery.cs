using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetWordContainingTitleQuery : IQuery<Page<Word>>
    {
        public string SearchTerm { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int DictionaryId { get; set; }
    }

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