using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals.Issues.Article
{
    public static class GetArticlesByIssue
    {
        [FunctionName("GetArticlesByIssue")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "periodicals/{periodicalId}/issue/{issueId}/articles")] HttpRequest req,
            ILogger log, int periodicalId, int issueId)
        {
            // parameters
            // query
            // pageNumber
            // pageSize
            // orderBy
             return new OkObjectResult($"GET:Articles for Issue {issueId} for Periodical {periodicalId}");
        }
    }
}
