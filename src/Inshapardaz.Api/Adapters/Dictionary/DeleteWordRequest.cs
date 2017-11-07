using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class DeleteWordRequest : DictionaryRequest
    {
        public int WordId { get; set; }
    }

    public class DeleteWordRequestHandler : RequestHandlerAsync<DeleteWordRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IAmACommandProcessor _commandProcessor;

        public DeleteWordRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DeleteWordRequest> HandleAsync(DeleteWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var word = await _queryProcessor.ExecuteAsync(new GetWordByIdQuery(command.DictionaryId, command.WordId), cancellationToken);

            if (word == null)
            {
                throw new NotFoundException();
            }

            await _commandProcessor.SendAsync(new DeleteWordCommand(command.DictionaryId, command.WordId), cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}