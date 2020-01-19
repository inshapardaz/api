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

namespace Inshapardaz.Functions.Library.Categories
{
    public class GetCategories : QueryBase
    {
        public GetCategories(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetCategories")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "categories")] HttpRequest req,
            ILogger log, [AccessToken] ClaimsPrincipal principal, 
            CancellationToken token)
        {
            var query = new GetCategoriesQuery();
            var categories = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            return new OkObjectResult(categories.Render(principal));
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("categories", relType);
    }
}
