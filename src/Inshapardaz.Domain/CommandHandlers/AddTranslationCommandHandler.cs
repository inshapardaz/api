using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddTranslationCommandHandler : RequestHandlerAsync<AddTranslationCommand>
    {
        private readonly ITranslationRepository _translationRepository;

        public AddTranslationCommandHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }
        
        public override async Task<AddTranslationCommand> HandleAsync(AddTranslationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            command.Result = await _translationRepository.AddTranslation(command.DictionaryId, command.WordId, command.Translation, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}