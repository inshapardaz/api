using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books.Chapters.Contents
{
    public class UpdateChapterContents :FunctionBase
    {
        public UpdateChapterContents(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("UpdateChapterContents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "books/{bookId}/chapter/{chapterId}/contents")] HttpRequest req,
            int bookId, int chapterId, 
            [AccessToken] ClaimsPrincipal principal = null,
            CancellationToken token = default(CancellationToken))
        {
            if (principal == null)
            {
                return new UnauthorizedResult();
            }

            if (!principal.IsWriter())
            {
                return new ForbidResult("Bearer");
            }

            var contents = await ReadBody(req);
            var contentType = GetHeader(req, "Accept", "text/markdown");

            var request = new UpdateChapterContentRequest(bookId, chapterId, contents, contentType);
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                if (request.Result.HasAddedNew)
                {
                    var renderResult = request.Result.ChapterContent.Render(principal);
                    return new CreatedResult(renderResult.Links.Self(), renderResult);
                }
                
                return new NoContentResult();
            }

            return new BadRequestResult();
        }

        public static LinkView Link(int bookId, int chapterId, string mimetype, string relType = RelTypes.Self) => SelfLink($"book/{bookId}/chapters/{chapterId}/contents", relType, "PUT", type: mimetype);
    }
}
