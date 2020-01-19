using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals.Issues.Article.Contents
{
    public static class AddArticleContents
    {
        [FunctionName("AddArticleContents")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "periodicals/{periodicalId}/issue/{issueId}/articles/{articleId}/contents")] HttpRequest req,
            ILogger log, int periodicalId, int issueId, int articleId)
        {
            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);
            return new OkObjectResult($"GET:Contents for Article {articleId} for Issue {issueId} for Periodical {periodicalId}");
        }
    }
}
