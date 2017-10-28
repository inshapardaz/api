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
    public class PutWordDetailRequest : DictionaryRequest
    {
        public long DetailId { get; set; }

        public WordDetailView WordDetail { get; set; }
    }

    public class PutWordDetailRequestHandler : RequestHandlerAsync<PutWordDetailRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public PutWordDetailRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PutWordDetailRequest> HandleAsync(PutWordDetailRequest command, CancellationToken cancellationToken = new CancellationToken())
        {

            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { DictionaryId = command.DictionaryId, WordDetailId = command.DetailId }, cancellationToken);

            if (details == null || details.Id != command.WordDetail.Id)
            {
                throw new BadRequestException();
            }

            var updateWordDetailCommand = new UpdateWordDetailCommand
            {
                DictionaryId = command.DictionaryId,
                WordId = details.WordInstanceId,
                WordDetail = command.WordDetail.Map<WordDetailView, WordDetail>()
            };

            await _commandProcessor.SendAsync(updateWordDetailCommand, cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
