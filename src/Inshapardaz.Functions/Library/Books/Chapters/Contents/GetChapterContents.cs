using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Extensions;
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
            ClaimsPrincipal principal = null,
            CancellationToken token = default)
        {
            return await Executor.Execute(async () =>
            {
                var contentType = GetHeader(req, "Accept", "text/markdown");
                var language = GetHeader(req, "Accept-Language", "");

                var query = new GetChapterContentQuery(libraryId, bookId, chapterId, language, contentType, principal.GetUserId())
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
