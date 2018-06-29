using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Ports.Dictionary;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class GetDownloadDictionaryRequest : DictionaryRequest
    {
        public GetDownloadDictionaryRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public string MimeType { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public byte[] Contents { get; set; }

            public string FileName { get; set; }
        }
    }

    public class GetDownloadDictionaryRequestHandler : RequestHandlerAsync<GetDownloadDictionaryRequest>
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public GetDownloadDictionaryRequestHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetDownloadDictionaryRequest> HandleAsync(GetDownloadDictionaryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var download = await _dictionaryRepository.GetDictionaryDownloadById(command.DictionaryId, command.MimeType, cancellationToken);

            if (download == null)
            {
                throw new NotFoundException();
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}