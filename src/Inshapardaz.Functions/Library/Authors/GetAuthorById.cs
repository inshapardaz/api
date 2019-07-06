using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Adapters.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Authors
{
    public class GetAuthorById : FunctionBase
    {
        private readonly IRenderAuthor _authorRenderer;
        public GetAuthorById(IAmACommandProcessor commandProcessor, IRenderAuthor authorRenderer)
        : base(commandProcessor)
        {
            _authorRenderer = authorRenderer;
        }

        [FunctionName("GetAuthorById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authors/{authorId}")] HttpRequest req,
            ILogger log, int authorId,
            [AccessToken] ClaimsPrincipal principal, 
            CancellationToken token)
        {

            var request = new GetAuthorByIdRequest(authorId);
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            return new OkObjectResult(_authorRenderer.Render(principal, request.Result));
        }

        public static LinkView Link(int authorId, string relType = RelTypes.Self) => SelfLink($"authors/{authorId}", relType, "GET");
    }
}
