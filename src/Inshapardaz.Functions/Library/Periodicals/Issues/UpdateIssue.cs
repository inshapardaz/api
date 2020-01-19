using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals.Issues
{
    public static class UpdateIssue
    {
        [FunctionName("UpdateIssue")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "periodicals/{periodicalId}/issues/{issueId}")] HttpRequest req,
            ILogger log, int periodicalId, int issueId)
        {
            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);
            return new OkObjectResult($"PUT:Issue {issueId} for Periodical {periodicalId}");
        }
    }
}
