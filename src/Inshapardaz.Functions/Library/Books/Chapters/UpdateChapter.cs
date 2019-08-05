using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books.Chapters
{
    public class UpdateChapter : FunctionBase
    {
        public UpdateChapter(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("UpdateChapter")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "books/{bookId}/chapter/{chapterId}")] ChapterView chapter,
            ILogger log, int bookId, int chapterId,
            [AccessToken] ClaimsPrincipal principal,
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

            var request = new UpdateChapterRequest(bookId, chapterId, chapter.Map());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = request.Result.Chapter.Render(principal);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new OkObjectResult(renderResult);
        }

        public static LinkView Link(int bookId, int chapterId, string relType = RelTypes.Self) => SelfLink($"book/{bookId}/chapters/{chapterId}", relType, "PUT");
    }
}
