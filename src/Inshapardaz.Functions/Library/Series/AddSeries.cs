using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Adapters.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Extentions;
using Inshapardaz.Functions.View.Library;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Series
{
    public class AddSeries : FunctionBase
    {
        private readonly IRenderSeries _seriesRenderer;

        public AddSeries(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator, IRenderSeries seriesRenderer)
        : base(commandProcessor, authenticator)
        {
            _seriesRenderer = seriesRenderer;
        }

        [FunctionName("AddSeries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "series")] HttpRequestMessage req,
            ILogger log, CancellationToken token)
        {
            var auth = await AuthenticateAsWriter(req, log);
            var category = await ReadBody<SeriesView>(req);

            var request = new AddSeriesRequest(category.Map<SeriesView, Domain.Entities.Library.Series>());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _seriesRenderer.Render(auth.User, request.Result);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("series", relType, "POST");
    }
}
