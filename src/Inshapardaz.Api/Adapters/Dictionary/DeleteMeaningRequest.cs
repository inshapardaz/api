using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Commands.Dictionary;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Queries.Dictionary;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class DeleteMeaningRequest : DictionaryRequest
    {
        public DeleteMeaningRequest(int dictionaryId, long wordId, long meaningId)
            : base(dictionaryId)
        {
            WordId = wordId;
            MeaningId = meaningId;
        }

        public long MeaningId { get; }

        public long WordId { get; }
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

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DeleteMeaningRequest> HandleAsync(DeleteMeaningRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var meaning = await _queryProcessor.ExecuteAsync(new GetWordMeaningByIdQuery(command.DictionaryId, command.WordId, command.MeaningId), cancellationToken);

            if (meaning == null)
            {
                throw new NotFoundException();
            }

            await _commandProcessor.SendAsync(new DeleteWordMeaningCommand(
                                                  command.DictionaryId, 
                                                  command.WordId,
                                                  meaning.Id), cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
