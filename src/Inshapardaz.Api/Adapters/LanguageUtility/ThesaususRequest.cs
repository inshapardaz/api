using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.LanguageUtility
{
    public class ThesaususRequest : IRequest
    {
        public Guid Id { get; set; }
        public int PageSize { get; internal set; }
        public int PageNumber { get; internal set; }
        public string Word { get; internal set; }
        public PageView<WordView> Response { get; internal set; }
    }

    public class ThesaususRequestHandler : RequestHandlerAsync<ThesaususRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderWordPage _pageRenderer;

        protected ThesaususRequestHandler(IQueryProcessor queryProcessor, IRenderWordPage pageRenderer)
        {
            _queryProcessor = queryProcessor;
            _pageRenderer = pageRenderer;
        }

        public async override Task<ThesaususRequest> HandleAsync(ThesaususRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = new GetWordRelationsByTitleQuery
            {
                PageSize = request.PageSize,
                PageNumber = request.PageNumber,
                Title = request.Word
            };
            var results = await _queryProcessor.ExecuteAsync(query, cancellationToken);
            var pageRenderArgs = new PageRendererArgs<Word>
            {
                RouteName = "GetWordAlternatives",
                Page = results
            };

            request.Response = _pageRenderer.Render(pageRenderArgs, -1);
            return await base.HandleAsync(request, cancellationToken);
        }
    }
}
