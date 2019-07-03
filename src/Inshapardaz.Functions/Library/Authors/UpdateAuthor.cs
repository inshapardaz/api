using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Adapters.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Authors
{
    public class UpdateAuthor : FunctionBase
    {
        private readonly IRenderAuthor _authorRenderer;
        public UpdateAuthor(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator, IRenderAuthor authorRenderer)
        : base(commandProcessor, authenticator)
        {
            _authorRenderer = authorRenderer;
        }

        [FunctionName("UpdateAuthor")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "authors/{authorId}")] HttpRequest req,
            ILogger log, int authorId, CancellationToken token)
        {
            var auth = await AuthenticateAsWriter(req, log);
            var author = await ReadBody<AuthorView>(req);
            author.Id = authorId;
            
            var request = new UpdateAuthorRequest(author.Map());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _authorRenderer.Render(auth.User, request.Result.Author);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }
            else
            {
                return new OkObjectResult(renderResult);
            }
        }

        public static LinkView Link(int authorId, string relType = RelTypes.Self) => SelfLink($"authors/{authorId}", relType, "PUT");
    }
}
