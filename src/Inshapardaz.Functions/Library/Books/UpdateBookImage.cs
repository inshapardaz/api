using System.Threading.Tasks;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books
{
    public class UpdateBookImage : FunctionBase
    {
        public UpdateBookImage(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator) 
        : base(commandProcessor, authenticator)
        {
        }

        [FunctionName("UpdateBookImage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "books/{bookId}/image")] HttpRequest req,
            ILogger log, int bookId)
        {
            return new OkObjectResult($"PUT:Book {bookId} image");
        }

        public static LinkView Link(int bookId, string relType = RelTypes.Self) => SelfLink($"books/{bookId}/image", relType, "PUT");
    }
}
