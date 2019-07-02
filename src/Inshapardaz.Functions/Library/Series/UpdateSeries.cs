using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Adapters.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Series
{
    public class UpdateSeries : FunctionBase
    {
        private readonly IRenderSeries _seriesRenderer;
        public UpdateSeries(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator, IRenderSeries seriesRenderer)
        : base(commandProcessor, authenticator)
        {
            _seriesRenderer = seriesRenderer;
        }

        [FunctionName("UpdateSeries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "series/{seriesId}")] HttpRequest req,
            ILogger log, int seriesId, CancellationToken token)
        {
            var auth = await AuthenticateAsWriter(req, log);
            var series = await ReadBody<SeriesView>(req);
            series.Id = seriesId;
            var request = new UpdateSeriesRequest(series.Map<SeriesView, Domain.Entities.Library.Series>());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _seriesRenderer.Render(auth.User, request.Result.Series);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }
            else
            {
                return new OkObjectResult(renderResult);
            }
        }

        public static LinkView Link(int seriesId, string relType = RelTypes.Self) => SelfLink($"series/{seriesId}", relType, "PUT");
    }
}
