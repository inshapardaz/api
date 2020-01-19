using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals.Issues.Article
{
    public static class GetArticleById
    {
        [FunctionName("GetArticleById")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "periodicals/{periodicalId}/issue/{issueId}/articles/{articleId}")] HttpRequest req,
            ILogger log, int periodicalId, int issueId, int articleId)
        {
            return new OkObjectResult($"GET:Article {articleId} for Issue {issueId} for Periodical {periodicalId}");
        }
    }
}
