using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Books.Chapters
{
    public static class DeleteChapter
    {
        [FunctionName("DeleteChapter")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "books/{bookId}/chapters/{chapterId}")] HttpRequest req,
            ILogger log, int bookId, int chapterId)
        {
            return new OkObjectResult($"DELETE:Chapter {chapterId} for Book {bookId}");
        }
    }
}
