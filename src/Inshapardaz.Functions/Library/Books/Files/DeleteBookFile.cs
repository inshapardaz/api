using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Books.Files
{
    public class DeleteBookFile : CommandBase
    {
        public DeleteBookFile(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("DeleteBookFile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "library/{libraryId}/book/{bookId}/files/{fileId}")] HttpRequest req,
            int libraryId,
            int bookId,
            int fileId,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token)
        {
            var request = new DeleteBookFileRequest(claims, libraryId, bookId, fileId, claims.GetUserId());
            await CommandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
