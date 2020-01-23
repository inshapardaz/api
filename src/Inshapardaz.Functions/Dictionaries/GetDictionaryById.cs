using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Inshapardaz.Functions.Views;
using Paramore.Darker;
using System.Security.Claims;
using System.Threading;
using Inshapardaz.Functions.Authentication;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Dictionaries;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;

namespace Inshapardaz.Functions.Dictionaries
{
    public class GetDictionaryById : QueryBase
    {
        public GetDictionaryById(IQueryProcessor queryProcessor)
            : base(queryProcessor)
        {
        }

        [FunctionName("GetDictionaryById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dictionaries/{dictionaryId:int}")] HttpRequest req,
            int dictionaryId,
            ILogger log,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            return await Action.Execute(async () =>
            {
                var query = new GetDictionaryByIdQuery(dictionaryId, principal.GetUserId());
                var author = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

                if (author != null)
                {
                    return new OkObjectResult(author.Render(principal));
                }

                return new NotFoundResult();
            }, principal);
        }

        public static LinkView Link(int dictionaryId, string relType = RelTypes.Self) => SelfLink($"dictionaries/{dictionaryId}", relType, "GET");
    }
}
