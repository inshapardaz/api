using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class DeleteDictionaryRequest : DictionaryRequest
    {
        public DeleteDictionaryRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }
    }

    public class DeleteDictionaryRequestHandler : RequestHandlerAsync<DeleteDictionaryRequest>
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public DeleteDictionaryRequestHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DeleteDictionaryRequest> HandleAsync(DeleteDictionaryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _dictionaryRepository.GetDictionaryById(command.DictionaryId, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException();
            }

            await _dictionaryRepository.DeleteDictionary(command.DictionaryId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
