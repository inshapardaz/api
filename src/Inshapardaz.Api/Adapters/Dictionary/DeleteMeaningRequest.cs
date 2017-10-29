using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class DeleteMeaningRequest : DictionaryRequest
    {
        public long MeaningId { get; set; }
    }

    public class DeleteMeaningRequestHandler : RequestHandlerAsync<DeleteMeaningRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public DeleteMeaningRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DeleteMeaningRequest> HandleAsync(DeleteMeaningRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _queryProcessor.ExecuteAsync(new GetWordMeaningByIdQuery
            {
                MeaningId = command.MeaningId
            }, cancellationToken);

            if (response == null)
            {
                throw new NotFoundException();
            }

            await _commandProcessor.SendAsync(new DeleteWordMeaningCommand
            {
                MeaningId = response.Id
            }, cancellationToken: cancellationToken);


            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
