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
    public class AddChapterContents : FunctionBase
    {
        public AddChapterContents(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("AddChapterContents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "books/{bookId}/chapters/{chapterId}/contents")] HttpRequest req,
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

            var request = new AddChapterContentRequest(bookId, chapterId, contents, contentType);
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var renderResult = request.Result.Render(principal);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new BadRequestResult();
        }

        public static LinkView Link(int bookId, int chapterId, string relType = RelTypes.Self) => SelfLink($"book/{bookId}/chapters/{chapterId}/contents", relType, "POST");
    }
}
