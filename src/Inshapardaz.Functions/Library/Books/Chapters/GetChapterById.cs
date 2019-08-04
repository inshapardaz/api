using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books.Chapters
{
    public class GetChapterById : FunctionBase
    {
        public GetChapterById(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("GetChapterById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "book/{bookId}/chapters/{chapterId}")] int bookId, int chapterId,
            ILogger log, ClaimsPrincipal principal, CancellationToken token)
        {
            var request = new GetChapterByIdRequest(bookId, chapterId);
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                return new OkObjectResult(request.Result.Render(principal));
            }

            return new NotFoundResult();
        }

        public static LinkView Link(int bookId, int chapterId, string relType = RelTypes.Self) => SelfLink($"book/{bookId}/chapters/{chapterId}", relType);

    }
}
