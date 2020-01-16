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
    public class GetLatestBooks : CommandBase
    {
        public GetLatestBooks(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("GetLatestBooks")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books/latest")] HttpRequest req,
            ILogger log, [AccessToken] ClaimsPrincipal principal, CancellationToken token)
        {
            var pageSize = GetQueryParameter(req, "pageSize", 10);

            var request = new GetLatestBooksRequest(principal.GetUserId());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            return new OkObjectResult(request.Result.Render(principal, Link));
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("books/latest", relType);
    }
}
