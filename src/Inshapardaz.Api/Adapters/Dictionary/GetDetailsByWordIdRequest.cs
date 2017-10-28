using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class GetDetailsByWordIdRequest : DictionaryRequest
    {
        public long WordId { get; set; }

        public List<WordDetailView> Result { get; set; }
    }

    public class GetDetailsByWordIdRequestHandler : RequestHandlerAsync<GetDetailsByWordIdRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderWordDetail _wordDetailRenderer;

        public GetDetailsByWordIdRequestHandler(IQueryProcessor queryProcessor, IRenderWordDetail wordDetailRenderer)
        {
            _queryProcessor = queryProcessor;
            _wordDetailRenderer = wordDetailRenderer;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetDetailsByWordIdRequest> HandleAsync(GetDetailsByWordIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var query = new WordDetailsByWordQuery
            {
                DictionaryId = command.DictionaryId,
                WordId = command.WordId
            };

            var wordDetailViews = await _queryProcessor.ExecuteAsync(query, cancellationToken);
            command.Result = wordDetailViews.Select(w => _wordDetailRenderer.Render(w)).ToList();
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}