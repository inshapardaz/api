using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordTranslationCommandHandler : RequestHandlerAsync<DeleteWordTranslationCommand>
    {
        private readonly ITranslationRepository _translationRepository;

        public DeleteWordTranslationCommandHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }
       
        public override async Task<DeleteWordTranslationCommand> HandleAsync(DeleteWordTranslationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _translationRepository.DeleteTranslation(command.DictionaryId, command.WordId, command.TranslationId, cancellationToken);
        
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}