using System;
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
    public class PostMeaningRequest : DictionaryRequest
    {
        public PostMeaningRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public long WordId { get; set; }

        public MeaningView Meaning { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public MeaningView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PostMeaningRequestHandler : RequestHandlerAsync<PostMeaningRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderMeaning _meaningRenderer;

        public PostMeaningRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IRenderMeaning meaningRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _meaningRenderer = meaningRenderer;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PostMeaningRequest> HandleAsync(PostMeaningRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var word = await _queryProcessor.ExecuteAsync(new GetWordByIdQuery(command.DictionaryId, command.WordId), cancellationToken);

            if (word == null)
            {
                throw new BadRequestException();
            }

            var addWOrdCommand = new AddMeaningCommand(command.DictionaryId, word.Id, command.Meaning.Map<MeaningView, Meaning>());
            await _commandProcessor.SendAsync(addWOrdCommand, cancellationToken: cancellationToken);
            var response = _meaningRenderer.Render(addWOrdCommand.Result, command.DictionaryId);
            command.Result.Location =  response.Links.Single(x => x.Rel == RelTypes.Self).Href;
            command.Result.Response =  response;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
