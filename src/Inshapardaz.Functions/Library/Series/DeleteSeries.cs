using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Series
{
    public class DeleteSeries : CommandBase
    {
        public DeleteSeries(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("DeleteSeries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "library/{libraryId}/series/{seriesId:int}")] HttpRequest req,
            int libraryId,
            int seriesId,
            ClaimsPrincipal claims,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var request = new DeleteSeriesRequest(claims, libraryId, seriesId);
                await CommandProcessor.SendAsync(request, cancellationToken: token);
                return new NoContentResult();
            });
        }

        public static LinkView Link(int libraryId, int seriesId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/series/{seriesId}", relType, "DELETE");
    }
}
