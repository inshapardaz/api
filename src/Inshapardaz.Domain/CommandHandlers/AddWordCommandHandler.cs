using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordCommandHandler : RequestHandlerAsync<AddWordCommand>
    {
        private readonly IWordRepository _wordRepository;

        public AddWordCommandHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        public override async Task<AddWordCommand> HandleAsync(AddWordCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (command.Word == null)
            {
                throw new BadRequestException();
            }

            command.Word.DictionaryId = command.DictionaryId;

            var newWord = await _wordRepository.AddWord(command.DictionaryId, command.Word, cancellationToken);
            command.Result = newWord;

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}