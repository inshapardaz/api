using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books.Chapters.Contents
{
    public class DeleteChapterContents : CommandBase
    {
        public DeleteChapterContents(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("DeleteChapterContents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "books/{bookId:int}/chapter/{chapterId:int}/contents/{contentId:int}")] HttpRequest req,
            int bookId, 
            int chapterId,
            int contentId,
        [AccessToken] ClaimsPrincipal principal = null,
            CancellationToken token = default(CancellationToken))
        {
            if (principal == null)
            {
                return new UnauthorizedResult();
            }

            if (!principal.IsWriter())
            {
                return new ForbidResult("Bearer");
            }

            var request = new DeleteChapterContentRequest(bookId, chapterId, contentId, principal.GetUserId());
            await CommandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        public static LinkView Link(int bookId, int chapterId, int contentId, string mimetype, string relType = RelTypes.Self) => SelfLink($"book/{bookId}/chapters/{chapterId}/contents/{contentId}", relType, "DELETE", type: mimetype);
    }
}
