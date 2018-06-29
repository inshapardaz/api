using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.IndexingService;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionary
{
    public class SearchWordRequest : DictionaryRequest
    {
        public SearchWordRequest(int dictionaryId, string query, int pageNumber, int pageSize)
            : base(dictionaryId)
        {
            Query = query;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public string Query { get; }

        public int PageSize { get; }

        public int PageNumber { get; }

        public Page<Word> Result { get; set; }
    }

    public class SearchWordRequestHandler : RequestHandlerAsync<SearchWordRequest>
    {
        private readonly IWordRepository _wordRepository;
        private readonly IReadDictionaryIndex _indexReader;

        public SearchWordRequestHandler(IWordRepository wordRepository, IReadDictionaryIndex indexReader)
        {
            _wordRepository = wordRepository;
            _indexReader = indexReader;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<SearchWordRequest> HandleAsync(SearchWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.Query))
            {
                throw new NotFoundException();
            }

            var words = _indexReader.Search(command.DictionaryId, command.Query);

            command.Result = await _wordRepository.GetWordsById(command.DictionaryId, words, command.PageNumber, command.PageSize, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}