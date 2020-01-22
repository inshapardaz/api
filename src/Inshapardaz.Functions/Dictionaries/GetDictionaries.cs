using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading;
using Inshapardaz.Functions.Authentication;
using System.Security.Claims;
using Inshapardaz.Domain.Ports.Dictionaries;
using Paramore.Darker;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Dictionaries
{
    public class GetDictionaries : QueryBase
    {
        public GetDictionaries(IQueryProcessor queryProcessor)
            : base(queryProcessor)
        {
        }

        [FunctionName("GetDictionaries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dictionaries")] HttpRequest req,
            ILogger log,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            var query = new GetDictionariesQuery() { UserId = principal.GetUserId() };
            var result = await QueryProcessor.ExecuteAsync(query);
            return new OkObjectResult(result.Render(principal));
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("dictionaries", relType);
    }
}
