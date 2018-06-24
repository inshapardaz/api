using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Commands.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Queries.Dictionary;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PutMeaningRequest : DictionaryRequest
    {
        public PutMeaningRequest(int dictionaryId, long wordId, int meaningId)
            : base(dictionaryId)
        {
            WordId = wordId;
            MeaningId = meaningId;
        }

        public int MeaningId { get; }

        public long WordId { get; }

        public MeaningView Meaning { get; set; }
    }

    public class PutMeaningRequestHandler : RequestHandlerAsync<PutMeaningRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IAmACommandProcessor _commandProcessor;

        public PutMeaningRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
            _commandProcessor = commandProcessor;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PutMeaningRequest> HandleAsync(PutMeaningRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _queryProcessor.ExecuteAsync(new GetWordMeaningByIdQuery(command.DictionaryId, command.WordId, command.MeaningId), cancellationToken);

            if (response == null || response.Id != command.Meaning.Id)
            {
                throw new BadRequestException();
            }

            await _commandProcessor.SendAsync(new UpdateWordMeaningCommand
            (
                command.DictionaryId,
                command.WordId,
                command.Meaning.Map<MeaningView, Meaning>()
            ), cancellationToken: cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
