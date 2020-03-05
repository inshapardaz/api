using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Books.Chapters
{
    public class DeleteChapter : CommandBase
    {
        public DeleteChapter(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("DeleteChapter")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "library/{libraryId}/books/{bookId:int}/chapters/{chapterId:int}")] HttpRequest req,
            int libraryId,
            int bookId,
            int chapterId,
            [AccessToken] ClaimsPrincipal claims,
            ILogger log,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var request = new DeleteChapterRequest(claims, libraryId, bookId, chapterId, claims.GetUserId());
                await CommandProcessor.SendAsync(request, cancellationToken: token);
                return new NoContentResult();
            });
        }

        public static LinkView Link(int bookId, int chapterId, string relType = RelTypes.Self) => SelfLink($"book/{bookId}/chapters/{chapterId}", relType, "DELETE");
    }
}
