using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionaries;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class UpdateTranslationRequest : DictionaryRequest
    {
        public UpdateTranslationRequest(int dictionaryId, long wordId, Translation translation)
            : base(dictionaryId)
        {
            WordId = wordId;
            Translation = translation;
        }

        public Translation Translation { get; set; }

        public long WordId { get; }

        public UpdateTranslationResult Result { get; set; } = new UpdateTranslationResult();

        public class UpdateTranslationResult
        {
            public Translation Translation { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateTranslationRequestHandler : RequestHandlerAsync<UpdateTranslationRequest>
    {
        private readonly ITranslationRepository _translationRepository;

        public UpdateTranslationRequestHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }
        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<UpdateTranslationRequest> HandleAsync(UpdateTranslationRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _translationRepository.GetTranslationById(command.DictionaryId, command.WordId, command.Translation.Id, cancellationToken);

            if (response == null)
            {
                command.Translation.Id = default(long);
                var newTranslation = await _translationRepository.AddTranslation(command.DictionaryId, command.WordId, command.Translation, cancellationToken);
                command.Result.HasAddedNew = true;
                command.Result.Translation = newTranslation;
            }
            else
            {
                await _translationRepository.UpdateTranslation(command.DictionaryId, command.WordId, command.Translation, cancellationToken);
                command.Result.Translation = command.Translation;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
