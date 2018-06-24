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
    public class AddTranslationCommand : Command
    {
        public AddTranslationCommand(int dictionaryId, long wordId, Translation translation)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            Translation = translation ?? throw new ArgumentNullException(nameof(translation));
        }

        public int DictionaryId { get; }

        public long WordId { get; }


        public Translation Translation { get;  }

        public Translation Result { get; set; }
    }

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