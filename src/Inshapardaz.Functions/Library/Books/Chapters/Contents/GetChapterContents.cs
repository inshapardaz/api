using System.Threading.Tasks;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books.Chapters.Contents
{
    public class GetChapterContents : FunctionBase
    {
        public GetChapterContents(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("GetChapterContents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "book/{bookId}/chapters/{chapterId}/contents")] HttpRequest req,
            ILogger log, int bookId, int chapterId)
        {
            return new OkObjectResult($"Get:Contents chapter {chapterId} of Book {bookId}");
        }

        public static LinkView Link(int bookId, int chapterId, string mimeType, string relType = RelTypes.Self) 
            => SelfLink($"book/{bookId}/chapters/{chapterId}/contents", relType, type: mimeType);
    }
}
