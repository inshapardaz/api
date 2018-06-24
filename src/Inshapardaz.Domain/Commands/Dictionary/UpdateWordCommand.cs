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
    public class UpdateWordCommand : Command
    {
        public int DictionaryId { get; }

        public UpdateWordCommand(int dictionaryId, Word word)
        {
            DictionaryId = dictionaryId;
            Word = word ?? throw new ArgumentNullException(nameof(word));
        }

        public Word Word { get; }
    }

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
