using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
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
    public class DeleteSeries : FunctionBase
    {
        public DeleteSeries(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator) 
        : base(commandProcessor, authenticator)
        {
        }

        [FunctionName("DeleteSeries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "series/{seriesId}")] HttpRequest req,
            ILogger log, int seriesId, CancellationToken token)
        {
            var auth = await AuthenticateAsWriter(req, log);

             var request = new DeleteSeriesRequest(seriesId);
            await CommandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        public static LinkView Link(int seriesId, string relType = RelTypes.Self) => SelfLink($"series/{seriesId}", relType, "DELETE");
    }
}
