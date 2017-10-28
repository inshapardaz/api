using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PutDictionaryRequest : DictionaryRequest
    {
        public DictionaryView Dictionary { get; set; }
        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public DictionaryView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PutDictionaryRequestHandler : RequestHandlerAsync<PutDictionaryRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public PutDictionaryRequestHandler(IAmACommandProcessor commandProcessor, 
                                           IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PutDictionaryRequest> HandleAsync(PutDictionaryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { DictionaryId = command.DictionaryId }, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException();
            }

            UpdateDictionaryCommand updateDictionaryCommand = new UpdateDictionaryCommand
            {
                Dictionary = command.Dictionary.Map<DictionaryView, Domain.Database.Entities.Dictionary>()
            };

            await _commandProcessor.SendAsync(updateDictionaryCommand, cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}