using System.Security.Claims;
using System.Threading;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books.Files
{
    public class GetBookFiles : FunctionBase
    {
        public GetBookFiles(IAmACommandProcessor commandProcessor) 
        : base(commandProcessor)
        {
        }

        [FunctionName("GetBookFiles")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books/{bookId}/files")] HttpRequest req,
            int bookId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            return new OkObjectResult("GET:Files for book {bookId}");
        }

        public static LinkView Link(int bookId, string relType = RelTypes.Self) => SelfLink($"books/{bookId}/files", relType);        
    }
}
