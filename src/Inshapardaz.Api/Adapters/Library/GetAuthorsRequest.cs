using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Library;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Queries.Library;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Library
{
    public class GetAuthorsRequest : RequestBase
    {
        public GetAuthorsRequest(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public PageView<AuthorView> Result { get; set; }
    }

    public class GetAuthorsRequestHandler : RequestHandlerAsync<GetAuthorsRequest>
    {
        private readonly IUserHelper _userHelper;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderAuthors _authorRenderer;

        public GetAuthorsRequestHandler(IQueryProcessor queryProcessor, IUserHelper userHelper, IRenderAuthors authorRenderer)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _authorRenderer = authorRenderer;
        }

        public override async Task<GetAuthorsRequest> HandleAsync(GetAuthorsRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var results = await _queryProcessor.ExecuteAsync(new GetAuthorsQuery {PageSize = command.PageSize, PageNumber = command.PageNumber}, cancellationToken);

            var pageRenderArgs = new PageRendererArgs<Author>
            {
                RouteName = "GetAuthors",
                Page = results
            };

            //TODO : Add count all logic
            command.Result = _authorRenderer.Render(pageRenderArgs, 0);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
