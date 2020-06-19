using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Darker;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Series
{
    public class GetSeriesById : QueryBase
    {
        public GetSeriesById(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetSeriesById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}series/{seriesId:int}")] HttpRequest req,
            int libraryId, int seriesId, [AccessToken] ClaimsPrincipal principal, CancellationToken token)
        {
            var query = new GetSeriesByIdQuery(libraryId, seriesId);
            var series = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (series == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(series.Render(libraryId, principal));
        }

        public static LinkView Link(int libraryId, int seriesId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/series/{seriesId}", relType);
    }
}
