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

namespace Inshapardaz.Api.Ports
{
    public class GetWordsStartingWithRequest : IRequest
    {
        public Guid Id { get; set; }

        public string StartingWith { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public int DictionaryId { get; set; }

        public PageView<WordView> Result { get; set; }
    }

    public class GetWordsStartingWithRequestHandler : RequestHandlerAsync<GetWordsStartingWithRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderWordPage _pageRenderer;

        public GetWordsStartingWithRequestHandler(IQueryProcessor queryProcessor, IRenderWordPage pageRenderer)
        {
            _queryProcessor = queryProcessor;
            _pageRenderer = pageRenderer;
        }

        public override async Task<GetWordsStartingWithRequest> HandleAsync(GetWordsStartingWithRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.StartingWith))
            {
                throw new NotFoundException();
            }

            var query = new WordStartingWithQuery
            {
                PageSize = command.PageSize,
                PageNumber = command.PageNumber,
                Title = command.StartingWith,
                DictionaryId = command.DictionaryId
            };
            var results = await _queryProcessor.ExecuteAsync(query, cancellationToken);

            var pageRenderArgs = new PageRendererArgs<Word>
            {
                RouteName = "GetWordsListStartWith",
                Page = results,
                RouteArguments = new RouteWithTitlePageRouteArgs
                {
                    Title = command.StartingWith
                }
            };

            command.Result =  _pageRenderer.Render(pageRenderArgs, -1);
            return command;
        }
    }
}