using Inshapardaz.Domain.Models.Library;
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

namespace Inshapardaz.Functions.Library.Books.Content
{
    public class DeleteBookContent : CommandBase
    {
        public DeleteBookContent(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("DeleteBookFile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "library/{libraryId}/book/{bookId}/content")] HttpRequest req,
            int libraryId,
            int bookId,
            ClaimsPrincipal claims,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var mimeType = GetHeader(req, "Accept", "text/markdown");
                var language = GetHeader(req, "Accept-Language", "");

                var request = new DeleteBookFileRequest(claims, libraryId, bookId, language, mimeType, claims.GetUserId());
                await CommandProcessor.SendAsync(request, cancellationToken: token);
                return new NoContentResult();
            });
        }

        public static LinkView Link(int libraryId, int bookId, string mimetype, string language, string relType = RelTypes.Self)
            => SelfLink($"library/{libraryId}/books/{bookId}/content", relType, "DELETE", media: mimetype, language: language);
    }
}
