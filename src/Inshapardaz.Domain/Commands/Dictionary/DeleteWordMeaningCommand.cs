using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Commands.Dictionary
{
    public class DeleteWordMeaningCommand : Command
    {
        public DeleteWordMeaningCommand(int dictionaryId, long wordId, long meaningId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            MeaningId = meaningId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public long MeaningId { get; }
    }

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