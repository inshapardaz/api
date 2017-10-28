using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class SearchWordRequest : IRequest
    {
        public Guid Id { get; set; }

        public string Query { get; set; }

        public int DictionaryId { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public PageView<WordView> Result { get; set; }
    }

    public class SearchWordRequestHandler : RequestHandlerAsync<SearchWordRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderWordPage _pageRenderer;

        public SearchWordRequestHandler(IQueryProcessor queryProcessor, IRenderWordPage pageRenderer)
        {
            _queryProcessor = queryProcessor;
            _pageRenderer = pageRenderer;
        }

        public override async Task<SearchWordRequest> HandleAsync(SearchWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.Query))
            {
                throw new NotFoundException();
            }

            var wordQuery = new WordContainingTitleQuery
            {
                DictionaryId = command.DictionaryId,
                SearchTerm = command.Query,
                PageNumber = command.PageNumber,
                PageSize = command.PageSize
            };

            var response = await _queryProcessor.ExecuteAsync(wordQuery, cancellationToken);
            var pageRenderArgs = new PageRendererArgs<Word>
            {
                RouteName = "SearchDictionary",
                Page = response,
                RouteArguments = new DictionarySearchPageRouteArgs
                {
                    Id = command.DictionaryId,
                    Query = command.Query
                }
            };

            command.Result = _pageRenderer.Render(pageRenderArgs, command.DictionaryId);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}