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
    public class AddMeaningCommand : Command
    {
        public AddMeaningCommand(int dictionaryId, long wordId, Meaning meaning)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            Meaning = meaning?? throw new ArgumentNullException(nameof(meaning));
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public Meaning Meaning { get; }

        public Meaning Result { get; set; }
    }

    public class AddMeaningCommandHandler : RequestHandlerAsync<AddMeaningCommand>
    {
        private readonly IMeaningRepository _meaningRepository;

        public AddMeaningCommandHandler(IMeaningRepository meaningRepository)
        {
            _meaningRepository = meaningRepository;
        }

        public override async Task<AddMeaningCommand> HandleAsync(AddMeaningCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            command.Meaning.WordId = command.WordId;
            var meaning = await _meaningRepository.AddMeaning(command.DictionaryId, command.WordId, command.Meaning, cancellationToken);

            command.Result = meaning;

            return await base.HandleAsync(command, cancellationToken);

        }
    }
}