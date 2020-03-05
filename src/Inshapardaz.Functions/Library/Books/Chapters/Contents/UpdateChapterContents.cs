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
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Books.Chapters.Contents
{
    public class UpdateChapterContents : CommandBase
    {
        public UpdateChapterContents(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("UpdateChapterContents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "library/{libraryId}/books/{bookId:int}/chapter/{chapterId:int}/contents")] HttpRequest req,
            int libraryId,
            int bookId, int chapterId,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token = default)
        {
            return await Executor.Execute(async () =>
            {
                var contents = await ReadBody(req);
                var contentType = GetHeader(req, "Accept", "text/markdown");

                var request = new UpdateChapterContentRequest(claims, libraryId, bookId, chapterId, contents, contentType, claims.GetUserId());
                await CommandProcessor.SendAsync(request, cancellationToken: token);

                if (request.Result != null)
                {
                    if (request.Result.HasAddedNew)
                    {
                        var renderResult = request.Result.ChapterContent.Render(claims);
                        return new CreatedResult(renderResult.Links.Self(), renderResult);
                    }

                    return new NoContentResult();
                }

                return new BadRequestResult();
            });
        }

        public static LinkView Link(int bookId, int chapterId, string mimetype, string relType = RelTypes.Self) => SelfLink($"book/{bookId}/chapters/{chapterId}/contents", relType, "PUT", type: mimetype);
    }
}
