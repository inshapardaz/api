using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals.Issues
{
    public static class DeleteIssue
    {
        [FunctionName("DeleteIssue")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "periodicals/{periodicalId}/issues/{issueId}")] HttpRequest req,
            ILogger log, int periodicalId, int issueId)
        {
            return new OkObjectResult($"DELETE:Issue {issueId} for Periodical {periodicalId}");
        }
    }
}
