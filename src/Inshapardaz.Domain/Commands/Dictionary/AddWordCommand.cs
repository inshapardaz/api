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
    public class AddWordCommand : Command
    {
        public AddWordCommand(int dictionaryId, Word word)
        {
            DictionaryId = dictionaryId;
            Word = word ?? throw new ArgumentNullException(nameof(word));
        }

        public int DictionaryId { get; }

        public Word Word { get; }

        public Word Result { get; set; }
    }

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
