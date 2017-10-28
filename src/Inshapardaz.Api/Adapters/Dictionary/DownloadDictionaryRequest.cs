using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class DownloadDictionaryRequest : DictionaryRequest
    {
        public string MimeType { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public byte[] Contents { get; set; }

            public string FileName { get; set; }
        }
    }

    public class DownloadDictionaryRequestHandler : RequestHandlerAsync<DownloadDictionaryRequest>
    {
        private readonly IQueryProcessor _queryProcessor;

        public DownloadDictionaryRequestHandler(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DownloadDictionaryRequest> HandleAsync(DownloadDictionaryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var query = new GetDownloadByDictionaryIdQuery
            {
                DictionaryId = command.DictionaryId,
                MimeType = command.MimeType
            };

            var file = await _queryProcessor.ExecuteAsync(query, cancellationToken);

            if (file == null)
            {
                throw new NotFoundException();
            }

            command.Result.FileName = file.FileName;
            command.Result.Contents = file.Contents;

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}