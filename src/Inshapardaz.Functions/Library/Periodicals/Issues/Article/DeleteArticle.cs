using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals.Issues.Article
{
    public static class DeleteArticle
    {
        [FunctionName("DeleteArticle")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "periodicals/{periodicalId}/issue/{issueId}/articles/{articleId}")] HttpRequest req,
            ILogger log, int periodicalId, int issueId, int articleId)
        {
            return new OkObjectResult($"DELETE:Article {articleId} for Issue {issueId} for Periodical {periodicalId}");
        }
    }
}
