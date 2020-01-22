using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Darker;

namespace Inshapardaz.Functions.Library.Books
{
    public class GetBooksByCategory : QueryBase
    {
        public GetBooksByCategory(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetBooksByCategory")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "categories/{categoryId:int}/books")] HttpRequest req,
            int categoryId, 
            [AccessToken] ClaimsPrincipal principal, 
            CancellationToken token)
        {
            var pageNumber = GetQueryParameter(req, "pageNumber", 1);
            var pageSize = GetQueryParameter(req, "pageSize", 10);

            var request = new GetBooksByCategoryQuery(categoryId, pageNumber, pageSize) { UserId = principal.GetUserId() };
            var books = await QueryProcessor.ExecuteAsync(request, cancellationToken: token);

            var args = new PageRendererArgs<BookModel>
            {
                Page = books,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                LinkFuncWithParameter = Link
            };

            return new OkObjectResult(args.Render(categoryId, principal));
        }

        public static LinkView Self(int categoryById, string relType = RelTypes.Self) => SelfLink($"categories/{categoryById}/books", relType);

        public static LinkView Link(int categoryById, int pageNumber = 1, int pageSize = 10, string relType = RelTypes.Self) 
            => SelfLink($"categories/{categoryById}/books", relType, queryString: new Dictionary<string, string>
            {
                { "pageNumber", pageNumber.ToString()},
                { "pageSize", pageSize.ToString()}
            });
    }
}
