using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
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
