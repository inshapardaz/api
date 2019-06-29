using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Books.Files
{
    public static class GetBookFiles
    {
        [FunctionName("GetBookFiles")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books/{bookId}/files")] HttpRequest req,
            ILogger log, int bookId)
        {
            return new OkObjectResult("GET:Files for book {bookId}");
        }
    }
}
