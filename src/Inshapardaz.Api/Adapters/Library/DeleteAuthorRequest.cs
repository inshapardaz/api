using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Domain.Helpers;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Library
{
    public class DeleteAuthorRequest : RequestBase
    {
        public DeleteAuthorRequest(int authorId)
        {
            AuthorId = authorId;
        }

        public int AuthorId { get; }
    }

    public class DeleteAuthorRequestHandler : RequestHandlerAsync<DeleteAuthorRequest>
    {
        private readonly IUserHelper _userHelper;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly ILogger<DeleteAuthorRequestHandler> _logger;

        public DeleteAuthorRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IUserHelper userHelper, ILogger<DeleteAuthorRequestHandler> logger)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _logger = logger;
        }

        public override async Task<DeleteAuthorRequest> HandleAsync(DeleteAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            //await _commandProcessor.SendAsync(new DeleteAuthorCommand (command.AuthorId), cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}