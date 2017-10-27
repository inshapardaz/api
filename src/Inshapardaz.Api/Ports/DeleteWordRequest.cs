using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class DeleteWordRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }

        public int WordId { get; set; }
    }

    public class DeleteWordRequestHandler : RequestHandlerAsync<DeleteWordRequest>
    {
        private readonly IUserHelper _userHelper;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly ILogger<DeleteDictionaryRequestHandler> _logger;

        public DeleteWordRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IUserHelper userHelper, ILogger<DeleteDictionaryRequestHandler> logger)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _logger = logger;
        }

        public override async Task<DeleteWordRequest> HandleAsync(DeleteWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var userId = _userHelper.GetUserId();
            if (userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException();

            }

            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { DictionaryId = command.DictionaryId }, cancellationToken);

            if (dictionary == null)
            {
                throw new NotFoundException();
            }

            if (!dictionary.IsPublic && dictionary.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }

            var word = await _queryProcessor.ExecuteAsync(new WordByIdQuery { WordId = command.WordId, UserId = userId }, cancellationToken);

            if (word == null)
            {
                throw new NotFoundException();
            }

            await _commandProcessor.SendAsync(new DeleteWordCommand {WordId = word.Id}, cancellationToken: cancellationToken);

            return command;
        }
    }
}