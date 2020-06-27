using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Darker;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Books.Chapters
{
    public class GetChapterById : QueryBase
    {
        public GetChapterById(IQueryProcessor queryProcessor)
            : base(queryProcessor)
        {
        }

        [FunctionName("GetChapterById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/book/{bookId:int}/chapters/{chapterId:int}")] HttpRequest req,
            int libraryId,
            int bookId,
            int chapterId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            var query = new GetChapterByIdQuery(libraryId, bookId, chapterId, principal.GetUserId());
            var chapter = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (chapter != null)
            {
                return new OkObjectResult(chapter.Render(libraryId, principal));
            }

            return new NotFoundResult();
        }

        public static LinkView Link(int libraryId, int bookId, int chapterId, string relType = RelTypes.Self)
            => SelfLink($"library/{libraryId}/books/{bookId}/chapters/{chapterId}", relType);
    }
}
