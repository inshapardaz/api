using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordCommandHandler : RequestHandlerAsync<DeleteWordCommand>
    {
        private readonly IWordRepository _wordRepository;

        public DeleteWordCommandHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        public override async Task<DeleteWordCommand> HandleAsync(DeleteWordCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _wordRepository.DeleteWord(command.DictionaryId, command.WordId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}