using System.Security.Claims;
using System.Threading;
using Inshapardaz.Functions.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books.Files
{
    public class DeleteBookFile : FunctionBase
    {
        public DeleteBookFile(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("DeleteBookFile")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "book/{bookId}/files/{fileId}")] HttpRequest req,
            int bookId, 
            int fileId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            return new OkObjectResult($"DELETE:File {fileId} for Book {bookId}");
        }
    }
}
