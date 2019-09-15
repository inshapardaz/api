using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books
{
    public class GetBooks : FunctionBase
    {
        public GetBooks(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("GetBooks")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books")] HttpRequest req,
            ILogger log, 
            [AccessToken] ClaimsPrincipal principal, 
            CancellationToken token)
        {
            var pageNumber = GetQueryParameter(req, "pageNumber", 1);
            var pageSize = GetQueryParameter(req, "pageSize", 10);
            
            var request = new GetBooksRequest(pageNumber, pageSize, principal.GetUserId());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var args = new Converters.PageRendererArgs<Book>
            {
                Page = request.Result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                LinkFunc = Link
            };
            
            return new OkObjectResult(args.Render(principal));
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("books", relType);

        public static LinkView Link(int pageNumber = 1, int pageSize = 10, string relType = RelTypes.Self) 
            => SelfLink("books", relType, queryString: new Dictionary<string, string>
            {
                { "pageNumber", pageNumber.ToString()},
                { "pageSize", pageSize.ToString()}
            });
    }
}
