using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Commands.Dictionary
{
    public class DeleteWordTranslationCommand : Command
    {
        public DeleteWordTranslationCommand(int dictionaryId, long wordId, long translationId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            TranslationId = translationId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public long TranslationId { get; }
    }

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