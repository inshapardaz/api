using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Files
{
    public static class DeleteFile
    {
        [FunctionName("DeleteFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "files/{id}")] HttpRequest req,
            ILogger log, int id)
        {
            return new OkObjectResult($"DELETE:File {id}");
        }
    }
}
