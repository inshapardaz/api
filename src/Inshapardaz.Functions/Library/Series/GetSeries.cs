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
    public class GetSeries : FunctionBase
    {
        private readonly IRenderSeriesList _seriesListRenderer;
        public GetSeries(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator, IRenderSeriesList seriesListRenderer)
        : base(commandProcessor, authenticator)
        {
            _seriesListRenderer = seriesListRenderer;
        }

        [FunctionName("GetSeries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series")] HttpRequest req,
            ILogger log, CancellationToken token)
        {
            var auth = await TryAuthenticate(req, log);

            var request = new GetSeriesRequest();
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            return new OkObjectResult(_seriesListRenderer.Render(auth?.User, request.Result));
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("series", relType);
    }
}
