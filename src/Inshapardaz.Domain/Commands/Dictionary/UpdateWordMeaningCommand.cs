using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Commands.Dictionary
{
    public class UpdateWordMeaningCommand : Command
    {
        public int DictionaryId { get; }

        public long WordId { get; }

        public UpdateWordMeaningCommand(int dictionaryId, long wordId, Meaning meaning)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            Meaning = meaning ?? throw new ArgumentNullException(nameof(meaning));
        }

        public Meaning Meaning { get; }
    }

    public class UpdateWordMeaningCommandHandler : RequestHandlerAsync<UpdateWordMeaningCommand>
    {
        private readonly IWordRepository _wordRepository;
        private readonly IMeaningRepository _meaningRepository;

        public UpdateWordMeaningCommandHandler(IWordRepository wordRepository, IMeaningRepository meaningRepository)
        {
            _meaningRepository = meaningRepository;
            _wordRepository = wordRepository;
        }

        public override async Task<UpdateWordMeaningCommand> HandleAsync(UpdateWordMeaningCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var word = await _wordRepository.GetWordById(command.DictionaryId, command.WordId, cancellationToken);

            if (word == null)
            {
                throw new NotFoundException();
            }

            var meaning = await _meaningRepository.GetMeaningById(command.DictionaryId, command.WordId, command.Meaning.Id, cancellationToken);
            if (meaning == null)
            {
                throw new NotFoundException();
            }

            await _meaningRepository.UpdateMeaning(command.DictionaryId, command.WordId, command.Meaning, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}