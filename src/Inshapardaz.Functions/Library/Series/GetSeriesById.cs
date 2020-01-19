using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Darker;

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series/{id:int}")] HttpRequest req,
            ILogger log, int id, [AccessToken] ClaimsPrincipal principal, CancellationToken token)
        {
            var query = new GetSeriesByIdQuery(id);
            var series = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (series == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(series.Render(principal));
        }

        public static LinkView Link(int seriesId, string relType = RelTypes.Self) => SelfLink($"series/{seriesId}", relType);
    }
}
