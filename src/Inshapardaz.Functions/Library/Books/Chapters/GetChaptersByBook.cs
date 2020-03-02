using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Darker;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Library.Books.Chapters
{
    public class GetChaptersByBook : QueryBase
    {
        public GetChaptersByBook(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetChaptersByBook")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/books/{bookId:int}/chapters")] HttpRequest req,
            int libraryId, int bookId, ClaimsPrincipal principal, CancellationToken token)
        {
            var query = new GetChaptersByBookQuery(libraryId, bookId, principal.GetUserId());
            var chapters = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (chapters != null)
            {
                return new OkObjectResult(chapters.Render(bookId, principal));
            }

            return new NotFoundResult();
        }

        public static LinkView Link(int bookId, string relType = RelTypes.Self) => SelfLink($"books/{bookId}/chapters", relType);
    }
}
