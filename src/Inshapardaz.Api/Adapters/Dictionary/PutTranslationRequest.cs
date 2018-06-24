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
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Queries.Dictionary;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PutTranslationRequest : DictionaryRequest
    {
        public PutTranslationRequest(int dictionaryId, long wordId, int translationId)
            : base(dictionaryId)
        {
            WordId = wordId;
            TranslationId = translationId;
        }

        public int TranslationId { get; }

        public TranslationView Translation { get; set; }

        public long WordId { get; }

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

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PutTranslationRequest> HandleAsync(PutTranslationRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _queryProcessor.ExecuteAsync(new GetTranslationByIdQuery(command.DictionaryId, command.WordId, command.TranslationId), cancellationToken);

            if (response == null)
            {
                throw new BadImageFormatException();
            }

            var updateCommand = new UpdateWordTranslationCommand(command.DictionaryId, command.WordId, command.Translation.Map<TranslationView, Translation>());
            await _commandProcessor.SendAsync(updateCommand, cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}