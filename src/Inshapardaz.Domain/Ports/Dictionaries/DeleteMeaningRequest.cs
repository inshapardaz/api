using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class DeleteMeaningRequest : DictionaryRequest
    {
        public DeleteMeaningRequest(int dictionaryId, long wordId, long meaningId)
            : base(dictionaryId)
        {
            WordId = wordId;
            MeaningId = meaningId;
        }

        public long MeaningId { get; }

        public long WordId { get; }
    }

    public class DeleteMeaningRequestHandler : RequestHandlerAsync<DeleteMeaningRequest>
    {
        private readonly IMeaningRepository _meaningRepository;

        public DeleteMeaningRequestHandler(IMeaningRepository meaningRepository)
        {
            _meaningRepository = meaningRepository;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DeleteMeaningRequest> HandleAsync(DeleteMeaningRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var meaning = await _meaningRepository.GetMeaningById(command.DictionaryId, command.WordId, command.MeaningId, cancellationToken);

            if (meaning == null)
            {
                throw new NotFoundException();
            }

            await _meaningRepository.DeleteMeaning(command.DictionaryId, 
                                                   command.WordId,
                                                   meaning.Id,
                                                   cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
