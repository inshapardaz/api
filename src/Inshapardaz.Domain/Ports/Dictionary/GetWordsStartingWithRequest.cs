using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionary
{
    public class GetWordsStartingWithRequest : DictionaryRequest
    {
        public GetWordsStartingWithRequest(int dictionaryId, string startingWith, int pageSize, int pageNumber)
            : base(dictionaryId)
        {
            StartingWith = startingWith;
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        public string StartingWith { get; }

        public int PageSize { get; }

        public int PageNumber { get; }

        public Page<Word> Result { get; set; }
    }

    public class GetWordsStartingWithRequestHandler : RequestHandlerAsync<GetWordsStartingWithRequest>
    {
        private readonly IWordRepository _wordRepository;

        public GetWordsStartingWithRequestHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetWordsStartingWithRequest> HandleAsync(GetWordsStartingWithRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.StartingWith))
            {
                throw new NotFoundException();
            }
            
            command.Result =  await _wordRepository.GetWordsStartingWith(command.DictionaryId, 
                                                                         command.StartingWith,
                                                                         command.PageNumber,
                                                                         command.PageSize, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}