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
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Series
{
    public class GetSeriesById : CommandBase
    {
        public GetSeriesById(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("GetSeriesById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series/{id:int}")] HttpRequest req,
            ILogger log, int id, [AccessToken] ClaimsPrincipal principal, CancellationToken token)
        {
            var request = new GetSeriesByIdRequest(id);
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(request.Result.Render(principal));
        }

        public static LinkView Link(int seriesId, string relType = RelTypes.Self) => SelfLink($"series/{seriesId}", relType);
    }
}
