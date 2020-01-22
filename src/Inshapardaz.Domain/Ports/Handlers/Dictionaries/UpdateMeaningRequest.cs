using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class UpdateMeaningRequest : DictionaryRequest
    {
        public UpdateMeaningRequest(int dictionaryId, long wordId, MeaningModel meaning)
            : base(dictionaryId)
        {
            WordId = wordId;
            Meaning = meaning;
        }

        public long WordId { get; }

        public MeaningModel Meaning { get; set; }

        public UpdateMeaningResult Result { get; set; } = new UpdateMeaningResult();

        public class UpdateMeaningResult
        {
            public MeaningModel Meaning { get; set; }


            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateMeaningRequestHandler : RequestHandlerAsync<UpdateMeaningRequest>
    {
        private readonly IMeaningRepository _meaningRepository;

        public UpdateMeaningRequestHandler(IMeaningRepository meaningRepository)
        {
            _meaningRepository = meaningRepository;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<UpdateMeaningRequest> HandleAsync(UpdateMeaningRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _meaningRepository.GetMeaningById(command.DictionaryId, command.WordId, command.Meaning.Id, cancellationToken);

            if (response == null)
            {
                command.Meaning.Id = default(long);
                var newMeaning = await _meaningRepository.AddMeaning(command.DictionaryId, command.WordId, command.Meaning, cancellationToken);
                command.Result.HasAddedNew = true;
                command.Result.Meaning = newMeaning;
            }
            else
            {
                await _meaningRepository.UpdateMeaning(command.DictionaryId, command.WordId, command.Meaning, cancellationToken);
                command.Result.Meaning = command.Meaning;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
