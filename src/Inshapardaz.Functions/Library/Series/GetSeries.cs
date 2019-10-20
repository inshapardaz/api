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
    // TODO : Add paging to series
    public class GetSeries : FunctionBase
    {
        public GetSeries(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("GetSeries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series")] HttpRequest req,
            ILogger log, [AccessToken] ClaimsPrincipal principal, CancellationToken token)
        {
            var request = new GetSeriesRequest();
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            return new OkObjectResult(request.Result.Render(principal));
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("series", relType);
    }
}
