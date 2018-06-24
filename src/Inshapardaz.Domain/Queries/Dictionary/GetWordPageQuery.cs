using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetWordPageQuery : IQuery<Page<Word>>
    {
        public GetWordPageQuery(int dictionaryId, int pageNumber, int pageSize)
        {
            DictionaryId = dictionaryId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int DictionaryId { get; }

        public int PageNumber { get; }

        public int PageSize { get; }
    }

    public class GetWordsPagesQueryHandler : QueryHandlerAsync<GetWordPageQuery, Page<Word>>
    {
        private readonly IWordRepository _wordRepository;

        public GetWordsPagesQueryHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        public override async Task<Page<Word>> ExecuteAsync(GetWordPageQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _wordRepository.GetWords(query.DictionaryId, query.PageNumber, query.PageSize, cancellationToken);
        }
    }
}