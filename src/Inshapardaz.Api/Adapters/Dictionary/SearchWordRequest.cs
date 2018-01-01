using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.IndexingService;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class SearchWordRequest : DictionaryRequest
    {
        public SearchWordRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public string Query { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public PageView<WordView> Result { get; set; }
    }

    public class SearchWordRequestHandler : RequestHandlerAsync<SearchWordRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderWordPage _pageRenderer;
        private readonly IReadDictionaryIndex _indexReader;

        public SearchWordRequestHandler(IQueryProcessor queryProcessor, IRenderWordPage pageRenderer, IReadDictionaryIndex indexReader)
        {
            _queryProcessor = queryProcessor;
            _pageRenderer = pageRenderer;
            _indexReader = indexReader;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<SearchWordRequest> HandleAsync(SearchWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.Query))
            {
                throw new NotFoundException();
            }

            var words = _indexReader.Search(command.DictionaryId, command.Query);
            var wordQuery = new GetWordsByIdsQuery(command.DictionaryId, words)
            {
                PageSize = command.PageSize,
                PageNumber = command.PageNumber
            };

            var response = await _queryProcessor.ExecuteAsync(wordQuery, cancellationToken);
            response.PageSize = command.PageNumber;
            response.PageSize = command.PageSize;

            var pageRenderArgs = new PageRendererArgs<Word>
            {
                RouteName = "SearchDictionary",
                Page = response,
                RouteArguments = new DictionarySearchPageRouteArgs
                {
                    Id = command.DictionaryId,
                    Query = command.Query,
                    PageNumber = command.PageNumber,
                    PageSize = command.PageSize
                }
            };

            command.Result = _pageRenderer.Render(pageRenderArgs, command.DictionaryId);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}