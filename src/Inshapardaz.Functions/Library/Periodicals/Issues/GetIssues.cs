using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals.Issues
{
    public static class GetIssues
    {
        [FunctionName("GetIssues")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "periodicals/{periodicalId}/issues")] HttpRequest req,
            ILogger log, int periodicalId)
        {
            return new OkObjectResult($"GET:Issues for Periodical {periodicalId}");
        }
    }
}
