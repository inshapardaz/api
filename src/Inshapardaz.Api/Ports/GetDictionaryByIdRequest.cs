using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class GetDictionaryByIdRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }

        public DictionaryView Result { get; set; }
    }

    public class GetDictionaryByIdRequestHandler : RequestHandlerAsync<GetDictionaryByIdRequest>
    {
        private readonly IUserHelper _userHelper;
        private readonly IRenderResponseFromObject<Dictionary, DictionaryView> _dictionaryRenderer;
        private readonly ILogger<GetDictionaryByIdRequestHandler> _logger;
        private readonly IQueryProcessor _queryProcessor;

        public GetDictionaryByIdRequestHandler(IQueryProcessor queryProcessor, 
                                               IUserHelper userHelper,
                                               IRenderResponseFromObject<Dictionary, DictionaryView> dictionaryRenderer,
                                               ILogger<GetDictionaryByIdRequestHandler> logger)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _dictionaryRenderer = dictionaryRenderer;
            _logger = logger;
        }

        public override async Task<GetDictionaryByIdRequest> HandleAsync(GetDictionaryByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var userId = _userHelper.GetUserId();
            var result = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { UserId = userId, DictionaryId = command.DictionaryId }, cancellationToken);

            if (result == null)
            {
                _logger.LogDebug("Unable to find dictionary with id {0}", command.DictionaryId);
                throw new NotFoundException();
            }
            
            command.Result = _dictionaryRenderer.Render(result);
            return command;
        }
    }
}
