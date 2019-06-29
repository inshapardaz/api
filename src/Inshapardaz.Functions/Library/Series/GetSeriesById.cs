using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Series
{
    public static class GetSeriesById
    {
        [FunctionName("GetSeriesById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series/{id}")] HttpRequest req,
            ILogger log, int id)
        {
            return new OkObjectResult($"Get:Series {id}");
        }
    }
}
