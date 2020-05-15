using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Books.Chapters
{
    public class UpdateChapter : CommandBase
    {
        public UpdateChapter(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("UpdateChapter")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "library/{libraryId}/books/{bookId:int}/chapter/{chapterId:int}")]
            ChapterView chapter,
            int libraryId,
            int bookId,
            int chapterId,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var request = new UpdateChapterRequest(claims, libraryId, bookId, chapterId, chapter.Map(), claims.GetUserId());
                await CommandProcessor.SendAsync(request, cancellationToken: token);

                var renderResult = request.Result.Chapter.Render(libraryId, claims);

                if (request.Result.HasAddedNew)
                {
                    return new CreatedResult(renderResult.Links.Self(), renderResult);
                }

                return new OkObjectResult(renderResult);
            });
        }

        public static LinkView Link(int libraryId, int bookId, int chapterId, string relType = RelTypes.Self)
            => SelfLink($"library/{libraryId}/books/{bookId}/chapters/{chapterId}", relType, "PUT");
    }
}
