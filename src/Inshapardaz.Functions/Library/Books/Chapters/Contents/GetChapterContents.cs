using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Darker;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Books.Chapters.Contents
{
    public class GetChapterContents : QueryBase
    {
        public GetChapterContents(IQueryProcessor queryProcessor)
            : base(queryProcessor)
        {
        }

        [FunctionName("GetChapterContents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/book/{bookId:int}/chapters/{chapterId:int}/contents")] HttpRequest req,
            int libraryId,
            int bookId,
            int chapterId,
            [AccessToken] ClaimsPrincipal principal = null,
            CancellationToken token = default)
        {
            var contentType = GetHeader(req, "Accept", "text/markdown");

            var query = new GetChapterContentQuery(libraryId, bookId, chapterId, contentType, principal.GetUserId())
            {
                UserId = principal.GetUserId()
            };

            var chapterContents = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (chapterContents != null)
            {
                return new OkObjectResult(chapterContents.Render(libraryId, principal));
            }

            return new NotFoundResult();
        }

        public static LinkView Link(int libraryId, int bookId, int chapterId, string mimeType, string relType = RelTypes.Self)
            => SelfLink($"library/{libraryId}/books/{bookId}/chapters/{chapterId}/contents", relType, type: mimeType);
    }
}
