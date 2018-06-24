using System;
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
    public class PutWordRequest : DictionaryRequest
    {
        public PutWordRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public WordView Word { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public int WordId { get; set; }

        public class RequestResult
        {
            public WordView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PutWordRequestHandler : RequestHandlerAsync<PutWordRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IAmACommandProcessor _commandProcessor;

        public PutWordRequestHandler(IQueryProcessor queryProcessor, 
                                      IAmACommandProcessor commandProcessor)
        {
            _queryProcessor = queryProcessor;
            _commandProcessor = commandProcessor;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PutWordRequest> HandleAsync(PutWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _queryProcessor.ExecuteAsync(new GetWordByIdQuery(command.DictionaryId, command.WordId), cancellationToken);

            if (response == null)
            {
                throw new NotFoundException();
            }

            var updateCommand = new UpdateWordCommand(command.DictionaryId, command.Word.Map<WordView, Word>());
            await _commandProcessor.SendAsync(updateCommand, cancellationToken: cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
