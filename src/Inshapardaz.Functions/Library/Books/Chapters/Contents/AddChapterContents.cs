using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;
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
    public class AddChapterContents : CommandBase
    {
        public AddChapterContents(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("AddChapterContents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "library/{libraryId}/books/{bookId:int}/chapters/{chapterId:int}/contents")] HttpRequest req,
            int libraryId,
            int bookId,
            int chapterId,
            // TODO : Make this work
            //[FromHeader("Accept", "text/markdown")] string contentType,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var contents = await ReadBody(req);
                var contentType = GetHeader(req, "Content-Type", "text/markdown");
                var language = GetHeader(req, "Accept-Language", "");

                var request = new AddChapterContentRequest(claims, libraryId, bookId, chapterId, contents, language, contentType, claims.GetUserId());
                await CommandProcessor.SendAsync(request, cancellationToken: token);

                if (request.Result != null)
                {
                    var renderResult = request.Result.Render(libraryId, claims);
                    return new CreatedResult(renderResult.Links.Self(), renderResult);
                }

                return new BadRequestResult();
            });
        }

        public static LinkView Link(int libraryId, int bookId, int chapterId, string relType = RelTypes.Self)
            => SelfLink($"library/{libraryId}/books/{bookId}/chapters/{chapterId}/contents", relType, "POST");
    }
}
