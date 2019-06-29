using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Authors
{
    public static class GetAuthorImage
    {
        [FunctionName("GetAuthorImage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authors/{id}/image")] HttpRequest req,
            ILogger log, int id)
        {
            return new OkObjectResult($"Get:Author {id} Image");
        }
    }
}
