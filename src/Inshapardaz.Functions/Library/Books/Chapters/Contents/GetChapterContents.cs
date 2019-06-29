using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Books.Chapters.Contents
{
    public static class GetChapterContents
    {
        [FunctionName("GetChapterContents")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "book/{bookId}/chapters/{chapterId}/contents")] HttpRequest req,
            ILogger log, int bookId, int chapterId)
        {
            return new OkObjectResult($"Get:Contents chapter {chapterId} of Book {bookId}");
        }
    }
}
