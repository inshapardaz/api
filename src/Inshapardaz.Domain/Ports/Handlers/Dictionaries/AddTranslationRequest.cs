using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class AddTranslationRequest : DictionaryRequest
    {
        public AddTranslationRequest(int dictionaryId, long wordId, Translation translation)
            : base(dictionaryId)
        {
            WordId = wordId;
            Translation = translation;
        }

        public Translation Translation { get; }

        public long WordId { get; }

        public Translation Result { get; set; }
    }

    public class AddTranslationRequestHandler : RequestHandlerAsync<AddTranslationRequest>
    {
        private readonly IWordRepository _wordRepository;
        private readonly ITranslationRepository _translationRepository;

        public AddTranslationRequestHandler(IWordRepository wordRepository, ITranslationRepository translationRepository)
        {
            _wordRepository = wordRepository;
            _translationRepository = translationRepository;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<AddTranslationRequest> HandleAsync(AddTranslationRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var word = await _wordRepository.GetWordById(command.DictionaryId, command.WordId, cancellationToken);

            if (word == null)
            {
                throw new BadRequestException();
            }

            command.Result = await _translationRepository.AddTranslation(command.DictionaryId, command.WordId, command.Translation, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
