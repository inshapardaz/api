using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class PutDictionaryRequest : IRequest
    {
        public Guid Id { get; set; }
        public int DictionaryId { get; set; }
        public DictionaryView Dictionary { get; set; }
        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public DictionaryView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PutDictionaryRequestHandler : RequestHandlerAsync<PutDictionaryRequest>
    {
        private readonly IUserHelper _userHelper;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ILogger<PutDictionaryRequestHandler> _logger;
        private readonly IRenderResponseFromObject<Dictionary, DictionaryView> _dictionaryRenderer;
        
        public PutDictionaryRequestHandler(IUserHelper userHelper, 
                                           IAmACommandProcessor commandProcessor, 
                                           IQueryProcessor queryProcessor,
                                           ILogger<PutDictionaryRequestHandler> logger, 
                                           IRenderResponseFromObject<Dictionary, DictionaryView> dictionaryRenderer)
        {
            _userHelper = userHelper;
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _logger = logger;
            _dictionaryRenderer = dictionaryRenderer;
        }
        public override async Task<PutDictionaryRequest> HandleAsync(PutDictionaryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var userId = _userHelper.GetUserId();

            var result = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { UserId = userId, DictionaryId = command.DictionaryId }, cancellationToken);

            if (result == null)
            {
                _logger.LogDebug("Existing dictionary not found. Creating new with name '{0}'", command.Dictionary.Name);
                var addDictionaryCommand = new AddDictionaryCommand
                {
                    Dictionary = command.Dictionary.Map<DictionaryView, Dictionary>()
                };

                addDictionaryCommand.Dictionary.UserId = userId;

                await _commandProcessor.SendAsync(addDictionaryCommand, cancellationToken: cancellationToken);

                var response = _dictionaryRenderer.Render(addDictionaryCommand.Dictionary);
                command.Result.Location = response.Links.Single(x => x.Rel == "self").Href;
                command.Result.Response = response;
                return command;
            }

            _logger.LogDebug("Updating existing dictionary with id '{0}'", command.DictionaryId);

            UpdateDictionaryCommand updateDictionaryCommand = new UpdateDictionaryCommand
            {
                Dictionary = command.Dictionary.Map<DictionaryView, Dictionary>()
            };

            updateDictionaryCommand.Dictionary.UserId = userId;

            await _commandProcessor.SendAsync(updateDictionaryCommand, cancellationToken: cancellationToken);

            return command;
        }
    }
}