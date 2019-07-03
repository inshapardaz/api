using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Adapters;
using Inshapardaz.Functions.Adapters.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Authors
{
    public class GetAuthors : FunctionBase
    {
        private readonly IRenderAuthors _authorsRenderer;

        public GetAuthors(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator, IRenderAuthors authorsRenderer) 
        : base(commandProcessor, authenticator)
        {
            _authorsRenderer = authorsRenderer;
        }

        [FunctionName("GetAuthors")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authors")] HttpRequest req,
            ILogger log, CancellationToken token)
        {
            
            var pageNumber = GetQueryParameter(req, "pageNumber", 1);
            var pageSize = GetQueryParameter(req, "pageSize", 10);
            
            var auth = await TryAuthenticate(req, log);

            var request = new GetAuthorsRequest(pageNumber, pageSize);
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var args = new PageRendererArgs<Author>
            {
                Page = request.Result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                LinkFunc = Link
            };
            
            return new OkObjectResult(_authorsRenderer.Render(auth?.User, args));
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("authors", relType);
        
        public static LinkView Link(int pageNumber = 1, int pageSize = 10, string relType = RelTypes.Self) 
            => SelfLink("authors", relType, queryString: new Dictionary<string, string>
            {
                { "pageNumber", pageNumber.ToString()},
                { "pageSize", pageSize.ToString()}
            });
    }
}
