using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordMeaningCommandHandler : RequestHandlerAsync<DeleteWordMeaningCommand>
    {
        private readonly IMeaningRepository _meaningRepository;

        public DeleteWordMeaningCommandHandler(IMeaningRepository meaningRepository)
        {
            _meaningRepository = meaningRepository;
        }

        public override async Task<DeleteWordMeaningCommand> HandleAsync(DeleteWordMeaningCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _meaningRepository.DeleteMeaning(command.DictionaryId, command.WordId, command.MeaningId, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}