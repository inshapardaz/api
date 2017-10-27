using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class CreateDictionaryDownloadRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();


        public class RequestResult
        {
            public DownloadDictionaryView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class CreateDictionaryDownloadRequestHandler : RequestHandlerAsync<CreateDictionaryDownloadRequest>
    {
        private readonly IUserHelper _userHelper;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderDictionaryDownload _dictionaryDownloadRenderer;
        private readonly ILogger<CreateDictionaryDownloadRequestHandler> _logger;

        public CreateDictionaryDownloadRequestHandler(IAmACommandProcessor commandProcessor, 
            IQueryProcessor queryProcessor, 
            IUserHelper userHelper, 
            IRenderDictionaryDownload dictionaryDownloadRenderer, 
            ILogger<CreateDictionaryDownloadRequestHandler> logger)
        {
            _userHelper = userHelper;
            _logger = logger;
            _dictionaryDownloadRenderer = dictionaryDownloadRenderer;
            _queryProcessor = queryProcessor;
            _commandProcessor = commandProcessor;
        }

        public override async Task<CreateDictionaryDownloadRequest> HandleAsync(CreateDictionaryDownloadRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var userId = _userHelper.GetUserId();
            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { UserId = userId, DictionaryId = command.DictionaryId }, cancellationToken);
            
            if (dictionary == null)
            {
                _logger.LogDebug("Dictionary with id '{0}' not found. Download cannot be created", command.DictionaryId);

                throw new  NotFoundException();
            }

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

            command.Result.Location = result.Links.Single(x => x.Rel == "self").Href;
            command.Result.Response = result;
            return command;
        }
    }
}