using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class DeleteWordDetailRequest : DictionaryRequest
    {
        public long DetailId { get; set; }
    }

    public class DeleteWordDetailRequestHandler : RequestHandlerAsync<DeleteWordDetailRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public DeleteWordDetailRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DeleteWordDetailRequest> HandleAsync(DeleteWordDetailRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery
            {
                DictionaryId = command.DictionaryId,
                WordDetailId = command.DetailId
            }, cancellationToken);

            if (details == null)
            {
                throw new NotFoundException();
            }

            await _commandProcessor.SendAsync(new DeleteWordDetailCommand
            {
                DictionaryId = command.DictionaryId,
                WordDetailId = command.DetailId
            }, cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
