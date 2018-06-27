using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionary
{
    public class AddWordRequest : DictionaryRequest
    {
        public AddWordRequest(int dictionaryId, Word word)
            : base(dictionaryId)
        {
            Word = word;
        }

        public Word Word { get; }

        public Word Result { get; set; }
    }

    public class AddWordRequestHandler : RequestHandlerAsync<AddWordRequest>
    {
        private readonly IWordRepository _wordRepository;

        public AddWordRequestHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<AddWordRequest> HandleAsync(AddWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _wordRepository.AddWord(command.DictionaryId, command.Word, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
