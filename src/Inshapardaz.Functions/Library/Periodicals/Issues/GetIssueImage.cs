using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals.Issues
{
    public static class GetIssueImage
    {
        [FunctionName("GetIssueImage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "periodicals/{periodicalId}/issues/{issueId}/image")] HttpRequest req,
            ILogger log, int periodicalId, int issueId)
        {
            return new OkObjectResult($"Get:Issue {issueId} Image ");
        }
    }
}
