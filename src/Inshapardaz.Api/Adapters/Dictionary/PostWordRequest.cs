﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Paramore.Brighter;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PostWordRequest : DictionaryRequest
    {
        public WordView Word { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public WordView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PostWordRequestHandler : RequestHandlerAsync<PostWordRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderWord _wordRenderer;

        public PostWordRequestHandler(IAmACommandProcessor commandProcessor,
                                      IRenderWord wordRenderer)
        {
            _wordRenderer = wordRenderer;
            _commandProcessor = commandProcessor;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PostWordRequest> HandleAsync(PostWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var addWordCommand = new AddWordCommand(command.DictionaryId, command.Word.Map<WordView, Word>());
            addWordCommand.Word.DictionaryId = command.DictionaryId;
            await _commandProcessor.SendAsync(addWordCommand, cancellationToken: cancellationToken);

            command.Result.Response = _wordRenderer.Render(addWordCommand.Word, command.DictionaryId);
            command.Result.Location = command.Result.Response.Links.Single(x => x.Rel == RelTypes.Self).Href;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}