﻿using System;
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
    public class PutWordRequest : DictionaryRequest
    {
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
                                      IAmACommandProcessor commandProcessor, 
                                      IUserHelper userHelper)
        {
            _queryProcessor = queryProcessor;
            _commandProcessor = commandProcessor;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
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