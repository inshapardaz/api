using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class AddWordRequest : DictionaryRequest
    {
        public AddWordRequest(int dictionaryId, WordModel word)
            : base(dictionaryId)
        {
            Word = word;
        }

        public WordModel Word { get; }

        public WordModel Result { get; set; }
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
