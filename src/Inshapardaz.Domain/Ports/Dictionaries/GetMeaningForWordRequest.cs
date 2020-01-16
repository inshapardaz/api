using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionaries;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class GetMeaningForWordRequest : DictionaryRequest
    {
        public GetMeaningForWordRequest(int dictionaryId, long wordId)
            : base(dictionaryId)
        {
            WordId = wordId;
        }

        public long WordId { get; }

        public IEnumerable<Meaning> Result { get; set; }
    }

    public class GetMeaningForWordRequestHandler : RequestHandlerAsync<GetMeaningForWordRequest>
    {
        private readonly IMeaningRepository _meaningRepository;

        public GetMeaningForWordRequestHandler(IMeaningRepository meaningRepository)
        {
            _meaningRepository = meaningRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetMeaningForWordRequest> HandleAsync(GetMeaningForWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _meaningRepository.GetMeaningByWordId(command.DictionaryId, command.WordId, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
