using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Books.Chapters
{
    public class AddChapter : CommandBase
    {
        public AddChapter(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("AddChapter")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "library/{libraryId}/books/{bookId:int}/chapters")]
            ChapterView chapter,
            int libraryId,
            int bookId,
            ClaimsPrincipal claims,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var request = new AddChapterRequest(claims, libraryId, bookId, chapter.Map(), claims.GetUserId());
                await CommandProcessor.SendAsync(request, cancellationToken: token);

                if (request.Result != null)
                {
                    var renderResult = request.Result.Render(libraryId, claims);
                    return new CreatedResult(renderResult.Links.Self(), renderResult);
                }

                return new BadRequestResult();
            });
        }

        public static LinkView Link(int libraryId, int bookId, string relType = RelTypes.Self)
            => SelfLink($"library/{libraryId}/books/{bookId}/chapters", relType, "POST");
    }
}
