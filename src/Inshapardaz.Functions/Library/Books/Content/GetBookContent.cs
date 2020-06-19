using Inshapardaz.Domain.Models.Library;
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

namespace Inshapardaz.Functions.Library.Books.Content
{
    public class GetBookContent : QueryBase
    {
        public GetBookContent(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetBookContent")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/books/{bookId}/content")] HttpRequest req,
            int libraryId,
            int bookId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var contentType = GetHeader(req, "Accept", "text/markdown");
                var language = GetHeader(req, "Accept-Language", "");

                var query = new GetBookContentQuery(libraryId, bookId, language, contentType, principal.GetUserId())
                {
                    UserId = principal.GetUserId()
                };

                var bookContents = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

                if (bookContents != null)
                {
                    return new OkObjectResult(bookContents.Render(libraryId, principal));
                }

                return new NotFoundResult();
            });
        }

        public static LinkView Link(int libraryId, int bookId, string language, string mimeType, string relType = RelTypes.Self)
            => SelfLink($"library/{libraryId}/books/{bookId}/content", relType, media: mimeType, language: language);
    }
}
