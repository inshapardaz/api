using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PostMeaningRequest : DictionaryRequest
    {
        public long DetailId { get; set; }

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

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PostMeaningRequest> HandleAsync(PostMeaningRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var detail = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { WordDetailId = command.DetailId }, cancellationToken);

            if (detail == null)
            {
                throw new BadRequestException();
            }

            var addWOrdCommand = new AddWordMeaningCommand { WordDetailId = detail.Id, Meaning = command.Meaning.Map<MeaningView, Meaning>() };
            await _commandProcessor.SendAsync(addWOrdCommand, cancellationToken: cancellationToken);
            var response = _meaningRenderer.Render(addWOrdCommand.Meaning);
            command.Result.Location =  response.Links.Single(x => x.Rel == "self").Href;
            command.Result.Response =  response;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
