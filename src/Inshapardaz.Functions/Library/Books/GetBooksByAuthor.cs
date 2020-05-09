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
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Books
{
    public class GetBooksByAuthor : QueryBase
    {
        public GetBooksByAuthor(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetBooksByAuthor")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/authors/{authorId:int}/books")] HttpRequest req,
            int libraryId,
            int authorId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            var pageNumber = GetQueryParameter(req, "pageNumber", 1);
            var pageSize = GetQueryParameter(req, "pageSize", 10);

            var query = new GetBooksByAuthorQuery(libraryId, authorId, pageNumber, pageSize, principal.GetUserId());
            var books = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            var args = new PageRendererArgs<BookModel>
            {
                Page = books,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                LinkFuncWithParameter = Link
            };

            return new OkObjectResult(args.Render(authorId, principal));
        }

        public static LinkView Link(int libraryId, int authorId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/authors/{authorId}/books", relType, "GET");

        public static LinkView Link(int authorId, int pageNumber = 1, int pageSize = 10, string query = null, string relType = RelTypes.Self)
            => SelfLink($"authors/{authorId}/books", relType, queryString: new Dictionary<string, string>
            {
                { "pageNumber", pageNumber.ToString()},
                { "pageSize", pageSize.ToString()}
            });
    }
}
