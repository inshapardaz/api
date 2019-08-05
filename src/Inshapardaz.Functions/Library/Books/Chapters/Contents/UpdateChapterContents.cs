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
    public class UpdateChapterContents :FunctionBase
    {
        public UpdateChapterContents(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("UpdateChapterContents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "books/{bookId}/chapter/{chapterId}/contents")] HttpRequest req,
            ILogger log, int bookId, int chapterId)
        {
            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);
            return new OkObjectResult($"PUT:Contents for Chapter {chapterId} for Book {bookId}");
        }

        public static LinkView Link(int bookId, int chapterId, string mimetype, string relType = RelTypes.Self) => SelfLink($"book/{bookId}/chapters/{chapterId}/contents", relType, "PUT", type: mimetype);
    }
}