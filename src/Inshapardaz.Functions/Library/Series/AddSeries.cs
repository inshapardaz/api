using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
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
    public class AddSeries : CommandBase
    {
        public AddSeries(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("AddSeries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "library/{libraryId}/series")]
            SeriesView series,
            int libraryId,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var request = new AddSeriesRequest(claims, libraryId, series.Map());
                await CommandProcessor.SendAsync(request, cancellationToken: token);

                var renderResult = request.Result.Render(libraryId, claims);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            });
        }

        public static LinkView Link(int libraryId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/series", relType, "POST");
    }
}
