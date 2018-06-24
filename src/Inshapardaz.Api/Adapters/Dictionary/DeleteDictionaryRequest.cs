using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Commands.Dictionary;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
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
        private readonly IUserHelper _userHelper;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly ILogger<DeleteDictionaryRequestHandler> _logger;

        public DeleteDictionaryRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IUserHelper userHelper, ILogger<DeleteDictionaryRequestHandler> logger)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _logger = logger;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DeleteDictionaryRequest> HandleAsync(DeleteDictionaryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _commandProcessor.SendAsync(new DeleteDictionaryCommand (command.DictionaryId), cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}