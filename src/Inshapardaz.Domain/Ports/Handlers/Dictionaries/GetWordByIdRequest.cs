using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class GetWordByIdRequest : DictionaryRequest
    {
        public GetWordByIdRequest(int dictionaryId, long wordId)
            : base(dictionaryId)
        {
            WordId = wordId;
        }

        public long WordId { get; }

        public WordModel Result { get; set; }
    }

    public class GetWordByIdRequestHandler : RequestHandlerAsync<GetWordByIdRequest>
    {
        private readonly IWordRepository _wordRepository;

        public GetWordByIdRequestHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetWordByIdRequest> HandleAsync(GetWordByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var word = await _wordRepository.GetWordById(command.DictionaryId, command.WordId, cancellationToken);
            if (word == null)
            {
                throw new NotFoundException();
            }

            command.Result = word;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
