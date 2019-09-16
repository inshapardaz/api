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
    public class UpdateBookFile : FunctionBase
    {
        public UpdateBookFile(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("UpdateBookFile")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "books/{bookId}/files/{fileId}")] HttpRequest req,
            int bookId, 
            int fileId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);
            return new OkObjectResult($"PUT:File {fileId} for Book {bookId}");
        }
    }
}
