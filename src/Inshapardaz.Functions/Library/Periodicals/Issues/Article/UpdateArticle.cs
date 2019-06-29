using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals.Issues.Article
{
    public static class UpdateArticle
    {
        [FunctionName("UpdateArticle")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "periodicals/{periodicalId}/issue/{issueId}/articles/{articleId}")] HttpRequest req,
            ILogger log, int periodicalId, int issueId, int articleId)
        {
            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);
            return new OkObjectResult($"PUT:Article {articleId} for Issue {issueId} for Periodical {periodicalId}");
        }
    }
}
