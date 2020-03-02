using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Darker;
using Inshapardaz.Functions.Authentication;
using System.Security.Claims;
using System.Threading;

namespace Inshapardaz.Functions.Dictionaries.Words
{
    public class GetWordById : QueryBase
    {
        public GetWordById(IQueryProcessor queryProcessor)
            : base(queryProcessor)
        {
        }

        [FunctionName("GetWordById")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dictionaries/{dictionaryId:int}/words/{wordId:long}")] HttpRequest req,
            int dictionaryId, long wordId,
            ILogger log,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            return new OkObjectResult($"GET:GetWordById({dictionaryId}, {wordId})");
        }
    }
}
