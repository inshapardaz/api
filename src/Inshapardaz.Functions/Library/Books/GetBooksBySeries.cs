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
    public class GetBooksBySeries : QueryBase
    {
        public GetBooksBySeries(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetBooksBySeries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/series/{seriesId:int}/books")] HttpRequest req,
            int libraryId,
            int seriesId,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token)
        {
            var pageNumber = GetQueryParameter(req, "pageNumber", 1);
            var pageSize = GetQueryParameter(req, "pageSize", 10);

            var query = new GetBooksBySeriesQuery(libraryId, seriesId, pageNumber, pageSize, claims.GetUserId());
            var books = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            var args = new PageRendererArgs<BookModel>
            {
                Page = books,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                LinkFuncWithParameterEx = Link
            };

            return new OkObjectResult(args.Render(seriesId, claims));
        }

        public static LinkView Link(int libraryId, int seriesId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/series/{seriesId}/books", relType);

        public static LinkView Link(int libraryId, int seriesId, int pageNumber = 1, int pageSize = 10, string query = null, string relType = RelTypes.Self)
            => SelfLink($"library/{libraryId}/series/{seriesId}/books", relType, queryString: new Dictionary<string, string>
            {
                { "pageNumber", pageNumber.ToString()},
                { "pageSize", pageSize.ToString()}
            });
    }
}
