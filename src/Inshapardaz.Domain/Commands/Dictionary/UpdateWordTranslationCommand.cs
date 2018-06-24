using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Commands.Dictionary
{
    public class UpdateWordTranslationCommand : Command
    {
        public int DictionaryId { get; }

        public UpdateWordTranslationCommand(int dictionaryId, long wordId, Translation translation)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            Translation = translation ?? throw new ArgumentNullException(nameof(translation));
        }

        public long WordId { get; }

        public Translation Translation { get; }
    }

    public class UpdateWordTranslationCommandHandler : RequestHandlerAsync<UpdateWordTranslationCommand>
    {
        private readonly ITranslationRepository _translationRepository;

        public UpdateWordTranslationCommandHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }

        public override async Task<UpdateWordTranslationCommand> HandleAsync(UpdateWordTranslationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _translationRepository.UpdateTranslation(command.DictionaryId, command.WordId, command.Translation, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
