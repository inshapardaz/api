using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Commands.Dictionary;
using Inshapardaz.Domain.Entities;
using Paramore.Brighter;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PostDictionaryDownloadRequest : DictionaryRequest
    {
        public PostDictionaryDownloadRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public DownloadDictionaryView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PostDictionaryDownloadRequestHandler : RequestHandlerAsync<PostDictionaryDownloadRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderDictionaryDownload _dictionaryDownloadRenderer;

        public PostDictionaryDownloadRequestHandler(IAmACommandProcessor commandProcessor,
            IRenderDictionaryDownload dictionaryDownloadRenderer)
        {
            _dictionaryDownloadRenderer = dictionaryDownloadRenderer;
            _commandProcessor = commandProcessor;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PostDictionaryDownloadRequest> HandleAsync(PostDictionaryDownloadRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var addDictionaryDownloadCommand = new AddDictionaryDownloadCommand
            {
                DitionarayId = command.DictionaryId,
                DownloadType = MimeTypes.SqlLite
            };

            await _commandProcessor.SendAsync(addDictionaryDownloadCommand, cancellationToken: cancellationToken);

            var result = _dictionaryDownloadRenderer.Render(new DownloadJobModel
            {
                Id = command.DictionaryId,
                Type = MimeTypes.SqlLite,
                JobId = addDictionaryDownloadCommand.JobId
            });

            command.Result.Location = result.Links.Single(x => x.Rel == RelTypes.Self).Href;
            command.Result.Response = result;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}