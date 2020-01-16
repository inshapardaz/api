using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionaries;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class GetWordsForDictionaryRequest : DictionaryRequest
    {
        public GetWordsForDictionaryRequest(int dictionaryId, int pageNumber, int pageSize)
            : base(dictionaryId)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; }

        public int PageSize { get; }

        public Page<Word> Result { get; set; }
    }

    public class GetWordsForDictionaryRequestHandler : RequestHandlerAsync<GetWordsForDictionaryRequest>
    {
        private readonly IWordRepository _wordRepository;

        public GetWordsForDictionaryRequestHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetWordsForDictionaryRequest> HandleAsync(GetWordsForDictionaryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _wordRepository.GetWords(command.DictionaryId, command.PageNumber, command.PageSize, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
