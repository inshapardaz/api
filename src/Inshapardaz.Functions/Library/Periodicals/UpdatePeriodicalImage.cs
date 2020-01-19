using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals
{
    public static class UpdatePeriodicalImage
    {
        [FunctionName("UpdatePeriodicalImage")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "periodicals/{periodicalId}/image")] HttpRequest req,
            ILogger log, int periodicalId)
        {
            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);
            return new OkObjectResult($"PUT:Periodical {periodicalId} Image");
        }
    }
}
