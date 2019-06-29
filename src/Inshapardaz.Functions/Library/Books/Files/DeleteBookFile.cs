using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Books.Files
{
    public static class DeleteBookFile
    {
        [FunctionName("DeleteBookFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "book/{bookId}/files/{fileId}")] HttpRequest req,
            ILogger log, int bookId, int fileId)
        {
            return new OkObjectResult($"DELETE:File {fileId} for Book {bookId}");
        }
    }
}
