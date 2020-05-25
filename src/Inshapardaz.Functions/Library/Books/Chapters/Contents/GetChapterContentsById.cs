using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
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
    public class GetChapterContentsById : QueryBase
    {
        public GetChapterContentsById(IQueryProcessor queryProcessor)
            : base(queryProcessor)
        {
        }

        [FunctionName("GetChapterContentsById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/book/{bookId:int}/chapters/{chapterId:int}/contents/{contentId:int}")] HttpRequest req,
            int libraryId,
            int bookId,
            int chapterId,
            int contentId,
            [AccessToken] ClaimsPrincipal principal = null,
            CancellationToken token = default)
        {
            return await Executor.Execute(async () =>
            {
                var query = new GetChapterContentByIdQuery(libraryId, bookId, chapterId, contentId, principal.GetUserId())
                {
                    UserId = principal.GetUserId()
                };

                var chapterContents = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

                if (chapterContents != null)
                {
                    return new OkObjectResult(chapterContents.Render(libraryId, principal));
                }

                return new NotFoundResult();
            });
        }

        public static LinkView Link(int libraryId, int bookId, int chapterId, int contentId, string relType = RelTypes.Self, string mimeType = null, string language = null)
            => SelfLink($"library/{libraryId}/books/{bookId}/chapters/{chapterId}/contents/{contentId}", relType, type: mimeType, language: language);
    }
}
