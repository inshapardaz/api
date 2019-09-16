using System;
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
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books.Chapters
{
    public class DeleteChapter : FunctionBase
    {
        public DeleteChapter(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("DeleteChapter")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "books/{bookId:int}/chapters/{chapterId:int}")] HttpRequest req,
            int bookId, int chapterId,
            [AccessToken] ClaimsPrincipal principal,
            ILogger log,
            CancellationToken token)
        {
            if (principal == null)
            {
                return new UnauthorizedResult();
            }

            if (!principal.IsWriter())
            {
                return new ForbidResult("Bearer");
            }

            var request = new DeleteChapterRequest(bookId, chapterId, principal.GetUserId());
            await CommandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        public static LinkView Link(int bookId, int chapterId, string relType = RelTypes.Self) => SelfLink($"book/{bookId}/chapters/{chapterId}", relType, "DELETE");
    }
}
