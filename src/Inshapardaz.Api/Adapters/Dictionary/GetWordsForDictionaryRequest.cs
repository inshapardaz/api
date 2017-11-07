using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class GetWordsForDictionaryRequest : DictionaryRequest
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public PageView<WordView> Result { get; set; }
    }

    public class GetWordsForDictionaryRequestHandler : RequestHandlerAsync<GetWordsForDictionaryRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderWordPage _pageRenderer;

        public GetWordsForDictionaryRequestHandler(IQueryProcessor queryProcessor, IRenderWordPage pageRenderer)
        {
            _queryProcessor = queryProcessor;
            _pageRenderer = pageRenderer;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetWordsForDictionaryRequest> HandleAsync(GetWordsForDictionaryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var query = new GetWordPageQuery(command.DictionaryId, command.PageNumber, command.PageSize);

            var results = await _queryProcessor.ExecuteAsync(query, cancellationToken);

            var pageRenderArgs = new PageRendererArgs<Word>
            {
                RouteName = "GetWords",
                Page = results
            };

            command.Result = _pageRenderer.Render(pageRenderArgs, command.DictionaryId);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
