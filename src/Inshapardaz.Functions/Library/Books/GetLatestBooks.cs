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

namespace Inshapardaz.Functions.Library.Books
{
    public class GetLatestBooks : FunctionBase
    {
        private readonly IRenderBooks _booksRenderer;
        public GetLatestBooks(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator, IRenderBooks booksRenderer)
        : base(commandProcessor, authenticator)
        {
            _booksRenderer = booksRenderer;
        }

        [FunctionName("GetLatestBooks")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books/latest")] HttpRequest req,
            ILogger log, CancellationToken token)
        {
            var pageSize = GetQueryParameter(req, "pageSize", 10);
            var auth = await TryAuthenticate(req, log);

            var request = new GetLatestBooksRequest();
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            return new OkObjectResult(_booksRenderer.Render(auth?.User, request.Result));
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("books/latest", relType);
    }
}
