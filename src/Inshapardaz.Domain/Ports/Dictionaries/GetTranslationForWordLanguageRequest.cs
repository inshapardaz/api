using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionaries;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class GetTranslationForWordLanguageRequest : DictionaryRequest
    {
        public GetTranslationForWordLanguageRequest(int dictionaryId, long wordId, Languages language)
            : base(dictionaryId)
        {
            WordId = wordId;
            Language = language;
        }

        public long WordId { get; }

        public Languages Language { get; }

        public IEnumerable<Translation> Result { get; set; }
    }

    public class GetTranslationForWordLanguageRequestHandler : RequestHandlerAsync<GetTranslationForWordLanguageRequest>
    {
        private readonly ITranslationRepository _translationRepository;

        public GetTranslationForWordLanguageRequestHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetTranslationForWordLanguageRequest> HandleAsync(GetTranslationForWordLanguageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _translationRepository.GetTranslationsByLanguage(command.DictionaryId, command.WordId, command.Language, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
