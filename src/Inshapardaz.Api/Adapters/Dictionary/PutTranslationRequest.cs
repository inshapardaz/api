using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PutTranslationRequest : DictionaryRequest
    {
        public int TranslationId { get; set; }
        public TranslationView Translation { get; set; }
        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public TranslationView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PutTranslationRequestHandler : RequestHandlerAsync<PutTranslationRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public PutTranslationRequestHandler(IAmACommandProcessor commandProcessor, 
                                           IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PutTranslationRequest> HandleAsync(PutTranslationRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _queryProcessor.ExecuteAsync(new GetTranslationByIdQuery { Id = command.TranslationId }, cancellationToken);

            if (response == null)
            {
                throw new BadImageFormatException();
            }

            var updateCommand = new UpdateWordTranslationCommand( command.DictionaryId, command.Translation.Map<TranslationView, Translation>());
            await _commandProcessor.SendAsync(updateCommand, cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}