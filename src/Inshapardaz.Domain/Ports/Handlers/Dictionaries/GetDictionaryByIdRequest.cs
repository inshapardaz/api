using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class GetDictionaryByIdRequest : DictionaryRequest
    {
        public GetDictionaryByIdRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public Models.Dictionaries.DictionaryModel Result { get; set; }
    }

    public class GetDictionaryByIdRequestHandler : RequestHandlerAsync<GetDictionaryByIdRequest>
    {
        private readonly IDictionaryRepository _dictionaryRepository;
        private readonly IWordRepository _wordRepository;

        public GetDictionaryByIdRequestHandler(IDictionaryRepository dictionaryRepository, IWordRepository wordRepository)
        {
            _dictionaryRepository = dictionaryRepository;
            _wordRepository = wordRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetDictionaryByIdRequest> HandleAsync(GetDictionaryByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var dictionary = await _dictionaryRepository.GetDictionaryById(command.DictionaryId, cancellationToken);

            if (dictionary == null)
            {
                throw new NotFoundException();
            }

            dictionary.WordCount = await _wordRepository.GetWordCountByDictionary(command.DictionaryId, cancellationToken);
            dictionary.Downloads = await _dictionaryRepository.GetDictionaryDownloads(command.DictionaryId, cancellationToken);
            command.Result = dictionary;

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
