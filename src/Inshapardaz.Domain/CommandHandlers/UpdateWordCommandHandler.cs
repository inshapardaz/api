using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordCommandHandler : RequestHandlerAsync<UpdateWordCommand>
    {
        private readonly IWordRepository _wordRepository;

        public UpdateWordCommandHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        public override async Task<UpdateWordCommand> HandleAsync(UpdateWordCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _wordRepository.UpdateWord(command.DictionaryId, command.Word, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}