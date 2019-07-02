using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Adapters.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Series
{
    public class GetSeriesById : FunctionBase
    {
        private readonly IRenderSeries _seriesRenderer;
        public GetSeriesById(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator, IRenderSeries seriesRenderer)
        : base(commandProcessor, authenticator)
        {
            _seriesRenderer = seriesRenderer;
        }

        [FunctionName("GetSeriesById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series/{id}")] HttpRequest req,
            ILogger log, int id, CancellationToken token)
        {
            var auth = await TryAuthenticate(req, log);

            var request = new GetSeriesByIdRequest(id);
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            return new OkObjectResult(_seriesRenderer.Render(auth?.User, request.Result));
        }

        public static LinkView Link(int seriesId, string relType = RelTypes.Self) => SelfLink($"series/{seriesId}", relType);
    }
}
