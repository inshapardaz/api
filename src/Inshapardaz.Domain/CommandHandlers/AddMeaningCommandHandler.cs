using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
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