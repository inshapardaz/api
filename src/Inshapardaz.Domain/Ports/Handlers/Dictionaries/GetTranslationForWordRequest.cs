using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class GetTranslationForWordRequest : DictionaryRequest
    {
        public GetTranslationForWordRequest(int dictionaryId, long wordId)
            : base(dictionaryId)
        {
            WordId = wordId;
        }

        public long WordId { get; set; }

        public IEnumerable<Translation> Result { get; set; }
    }

    public class GetTranslationForWordRequestHandler : RequestHandlerAsync<GetTranslationForWordRequest>
    {
        private readonly ITranslationRepository _translationRepository;

        public GetTranslationForWordRequestHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetTranslationForWordRequest> HandleAsync(GetTranslationForWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _translationRepository.GetTranslationsByWordId(command.DictionaryId, command.WordId, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
