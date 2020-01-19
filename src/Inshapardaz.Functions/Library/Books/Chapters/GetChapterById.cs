using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Darker;

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "book/{bookId:int}/chapters/{chapterId:int}")] HttpRequest req,
            int bookId, int chapterId,
            ILogger log, ClaimsPrincipal principal, CancellationToken token)
        {
            var query = new GetChapterByIdQuery(bookId, chapterId, principal.GetUserId());
            var chapter = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (chapter != null)
            {
                return new OkObjectResult(chapter.Render(principal));
            }

            return new NotFoundResult();
        }

        public static LinkView Link(int bookId, int chapterId, string relType = RelTypes.Self) => SelfLink($"book/{bookId}/chapters/{chapterId}", relType);

    }
}
