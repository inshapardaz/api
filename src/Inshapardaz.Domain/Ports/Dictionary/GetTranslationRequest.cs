using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionary
{
    public class GetTranslationRequest : DictionaryRequest
    {
        public GetTranslationRequest(int dictionaryId, long wordId, int translationId)
            : base(dictionaryId)
        {
            WordId = wordId;
            TranslationId = translationId;
        }

        public long WordId { get; }

        public long TranslationId { get; }

        public Translation Result { get; set; }
    }

    public class GetTranslationRequestHandler : RequestHandlerAsync<GetTranslationRequest>
    {
        private readonly ITranslationRepository _translationRepository;

        public GetTranslationRequestHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetTranslationRequest> HandleAsync(GetTranslationRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result =  await _translationRepository.GetTranslationById(command.DictionaryId, command.WordId, command.TranslationId, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
