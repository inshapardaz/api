using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Categories
{
    public static class GetCategoryById
    {
        [FunctionName("GetCategoryById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "categories/{id}")] HttpRequest req,
            ILogger log, int id)
        {
            return new OkObjectResult($"Get:Category {id}");
        }
    }
}
