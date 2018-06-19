﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PostTranslationRequest : DictionaryRequest
    {
        public PostTranslationRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public TranslationView Translation { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public long WordId { get; set; }

        public class RequestResult
        {
            public TranslationView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PostTranslationRequestHandler : RequestHandlerAsync<PostTranslationRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderTranslation _translationRenderer;

        public PostTranslationRequestHandler(IAmACommandProcessor commandProcessor, 
            IQueryProcessor queryProcessor,
            IRenderTranslation translationRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _translationRenderer = translationRenderer;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PostTranslationRequest> HandleAsync(PostTranslationRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var word = await _queryProcessor.ExecuteAsync(new GetWordByIdQuery(command.DictionaryId, command.WordId), cancellationToken);

            if (word == null)
            {
                throw new BadRequestException();
            }

            var addCommand = new AddTranslationCommand(
                command.DictionaryId,
                command.WordId,
                command.Translation.Map<TranslationView, Translation>()
            );
            await _commandProcessor.SendAsync(addCommand, cancellationToken: cancellationToken);

            var response = _translationRenderer.Render(addCommand.Result, command.DictionaryId);
            command.Result.Location =  response.Links.Single(x => x.Rel == RelTypes.Self).Href;
            command.Result.Response =  response;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
