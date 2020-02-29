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
    // TODO : Convert into paged endpoint
    public class GetSeries : QueryBase
    {
        public GetSeries(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetSeries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/series")] HttpRequest req,
            ILogger log, int libraryId, [AccessToken] ClaimsPrincipal principal, CancellationToken token)
        {
            var query = new GetSeriesQuery(libraryId);
            var series = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            return new OkObjectResult(series.Render(principal));
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("series", relType);
    }
}
