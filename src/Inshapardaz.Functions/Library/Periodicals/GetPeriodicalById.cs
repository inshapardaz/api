using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals
{
    public static class GetPeriodicalById
    {
        [FunctionName("GetPeriodicalById")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "periodicals/{periodicalId}")] HttpRequest req,
            ILogger log, int periodicalId)
        {
            return new OkObjectResult($"Get:Periodical {periodicalId}");
        }
    }
}
