using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books
{
    public class GetBookById : FunctionBase
    {
        public GetBookById(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("GetBookById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books/{bookId}")] HttpRequest req,
            ILogger log, int bookId,
            [AccessToken] ClaimsPrincipal principal, 
            CancellationToken token)
        {
            var request = new GetBookByIdRequest(bookId, principal.GetUserId());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                return new OkObjectResult(request.Result.Render(principal));
            }

            return new NotFoundResult();
        }

        public static LinkView Link(int bookId, string relType = RelTypes.Self) => SelfLink($"books/{bookId}", relType);
    }
}
