using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionary
{
    public class UpdateWordRequest : DictionaryRequest
    {
        public UpdateWordRequest(int dictionaryId, Word word)
            : base(dictionaryId)
        {
            Word = word;
        }

        public Word Word { get; set; }

        public UpdateWordResult Result { get; set; } = new UpdateWordResult();

        public class UpdateWordResult
        {
            public Word Word { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateWordRequestHandler : RequestHandlerAsync<UpdateWordRequest>
    {
        private readonly IWordRepository _wordRepository;

        public UpdateWordRequestHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }
        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<UpdateWordRequest> HandleAsync(UpdateWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _wordRepository.GetWordById(command.DictionaryId, command.Word.Id, cancellationToken);

            if (response == null)
            {
                command.Word.Id = default(long);
                var newWord = await _wordRepository.AddWord(command.DictionaryId, command.Word, cancellationToken);
                command.Result.HasAddedNew = true;
                command.Result.Word = newWord;
            }
            else
            {
                await _wordRepository.UpdateWord(command.DictionaryId, command.Word, cancellationToken);
                command.Result.Word = command.Word;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
