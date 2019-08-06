using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals.Issues
{
    public static class GetIssueById
    {
        [FunctionName("GetIssueById")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "periodicals/{periodicalId}/issue/{issueId}")] HttpRequest req,
            ILogger log, int periodicalId, int issueId)
        {
            return new OkObjectResult($"Get:Issue {issueId} for Periodical {periodicalId}");
        }
    }
}
