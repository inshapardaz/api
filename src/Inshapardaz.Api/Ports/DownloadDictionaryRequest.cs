using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class DownloadDictionaryRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }

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
        private readonly IUserHelper _userHelper;

        public DownloadDictionaryRequestHandler(IQueryProcessor queryProcessor, IUserHelper userHelper)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
        }

        public override async Task<DownloadDictionaryRequest> HandleAsync(DownloadDictionaryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var userId = _userHelper.GetUserId();
            var query = new GetDownloadByDictionaryIdQuery
            {
                DictionaryId = command.DictionaryId,
                UserId = userId,
                MimeType = command.MimeType
            };

            var file = await _queryProcessor.ExecuteAsync(query, cancellationToken);

            if (file == null)
            {
                throw new NotFoundException();
            }

            command.Result.FileName = file.FileName;
            command.Result.Contents = file.Contents;

            return command;
        }
    }
}