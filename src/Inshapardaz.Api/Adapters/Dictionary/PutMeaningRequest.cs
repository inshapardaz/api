using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PutMeaningRequest : DictionaryRequest
    {
        public long MeaningId { get; set; }

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

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PutMeaningRequest> HandleAsync(PutMeaningRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _queryProcessor.ExecuteAsync(new GetWordMeaningByIdQuery(command.MeaningId), cancellationToken);

            if (response == null || response.Id != command.Meaning.Id)
            {
                throw new BadRequestException();
            }

            await _commandProcessor.SendAsync(new UpdateWordMeaningCommand
            (
                command.DictionaryId,
                command.Meaning.Map<MeaningView, Meaning>()
            ), cancellationToken: cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
