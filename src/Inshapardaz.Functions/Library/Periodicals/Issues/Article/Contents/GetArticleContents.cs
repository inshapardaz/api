using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals.Issues.Article.Contents
{
    public static class GetArticleContents
    {
        [FunctionName("GetArticleContents")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "periodicals/{periodicalId}/issue/{issueId}/articles/{articleId}/content")] HttpRequest req,
            ILogger log, int periodicalId, int issueId, int articleId)
        {
            return new OkObjectResult($"GET: Content for article {articleId} for Issue {issueId} for Periodical {periodicalId}");
        }
    }
}
