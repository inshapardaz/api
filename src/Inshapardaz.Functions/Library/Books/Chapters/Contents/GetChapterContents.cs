using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "book/{bookId}/chapters/{chapterId}/contents")] 
            HttpRequest req,
            int bookId, 
            int chapterId,
            [AccessToken] ClaimsPrincipal principal = null,
            CancellationToken token = default(CancellationToken))
        {
            var contentType = GetHeader(req, "Accept", "text/markdown");

            var request = new GetChapterContentRequest(bookId, chapterId, contentType);
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                return new OkObjectResult(request.Result.Render(principal));
            }

            return new NotFoundResult();
        }

        public static LinkView Link(int bookId, int chapterId, string mimeType, string relType = RelTypes.Self) 
            => SelfLink($"book/{bookId}/chapters/{chapterId}/contents", relType, type: mimeType);
    }
}
