using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Darker;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Series
{
    public class GetSeries : QueryBase
    {
        public GetSeries(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetSeries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/series")] HttpRequest req,
            int libraryId,
            ClaimsPrincipal principal,
            CancellationToken token)
        {
            var query = GetQueryParameter<string>(req, "query", null);
            var pageNumber = GetQueryParameter(req, "pageNumber", 1);
            var pageSize = GetQueryParameter(req, "pageSize", 10);

            var seriesQuery = new GetSeriesQuery(libraryId, pageNumber, pageSize) { Query = query };
            var series = await QueryProcessor.ExecuteAsync(seriesQuery, cancellationToken: token);

            var args = new PageRendererArgs<SeriesModel>
            {
                Page = series,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
                LinkFuncWithParameter = Link
            };

            return new OkObjectResult(args.Render(libraryId, principal));
        }

        public static LinkView Link(int libraryId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/series", relType);

        public static LinkView Link(int libraryId, int pageNumber = 1, int pageSize = 10, string query = null, string relType = RelTypes.Self)
        {
            var queryString = new Dictionary<string, string>
            {
                { "pageNumber", pageNumber.ToString()},
                { "pageSize", pageSize.ToString()}
            };

            if (query != null)
            {
                queryString.Add("query", query);
            }

            return SelfLink($"library/{libraryId}/series", relType, queryString: queryString);
        }
    }
}
