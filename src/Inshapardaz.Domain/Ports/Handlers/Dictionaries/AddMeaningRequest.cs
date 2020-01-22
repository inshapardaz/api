using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class AddMeaningRequest : DictionaryRequest
    {
        public AddMeaningRequest(int dictionaryId, long wordId, MeaningModel meaning)
            : base(dictionaryId)
        {
            WordId = wordId;
            Meaning = meaning;
        }

        public long WordId { get; }

        public MeaningModel Meaning { get; }

        public MeaningModel Result { get; set; }
    }

    public class AddMeaningRequestHandler : RequestHandlerAsync<AddMeaningRequest>
    {
        private readonly IWordRepository _wordRepository;
        private readonly IMeaningRepository _meaningRepository;

        public AddMeaningRequestHandler(IWordRepository wordRepository, IMeaningRepository meaningRepository)
        {
            _wordRepository = wordRepository;
            _meaningRepository = meaningRepository;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<AddMeaningRequest> HandleAsync(AddMeaningRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var word = await _wordRepository.GetWordById(command.DictionaryId, command.WordId, cancellationToken);

            if (word == null)
            {
                throw new BadRequestException();
            }

            command.Result = await _meaningRepository.AddMeaning(command.DictionaryId, command.WordId, command.Meaning, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
