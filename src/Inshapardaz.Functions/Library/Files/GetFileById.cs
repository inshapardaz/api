using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Files
{
    public static class GetFileById
    {
        [FunctionName("GetFileById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "files/{fileId}.{extention?}")] HttpRequest req,
            ILogger log, int fileId, string extention)
        {
            // parameters 
            // height = 200
            // width = 200, 
            return new OkObjectResult($"Get:Series {fileId}.{extention}");
        }
    }
}
