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
    public class PostWordDetailRequest : DictionaryRequest
    {
        public long WordId { get; set; }

        public WordDetailView WordDetail { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public WordDetailView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PostWordDetailRequestHandler : RequestHandlerAsync<PostWordDetailRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderWordDetail _wordDetailRenderer;

        public PostWordDetailRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IRenderWordDetail wordDetailRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _wordDetailRenderer = wordDetailRenderer;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PostWordDetailRequest> HandleAsync(PostWordDetailRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _queryProcessor.ExecuteAsync(new WordByIdQuery { DictionaryId = command.DictionaryId, WordId = command.WordId }, cancellationToken);

            if (response == null)
            {
                throw new BadRequestException();
            }

            var addWordDetailCommand = new AddWordDetailCommand
            {
                DictionaryId = command.DictionaryId,
                WordId = command.WordId,
                WordDetail = command.WordDetail.Map<WordDetailView, WordDetail>()
            };

            await _commandProcessor.SendAsync(addWordDetailCommand, cancellationToken: cancellationToken);

            var responseView = _wordDetailRenderer.Render(addWordDetailCommand.WordDetail);
            command.Result.Location = responseView.Links.Single(x => x.Rel == "self").Href;
            command.Result.Response = responseView;

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
