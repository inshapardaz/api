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

namespace Inshapardaz.Functions.Library.Books
{
    public class UpdateBook : FunctionBase
    {
        private readonly IRenderBook _bookRenderer;

        public UpdateBook(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator, IRenderBook bookRenderer)
        : base(commandProcessor, authenticator)
        {
            _bookRenderer = bookRenderer;
        }

        [FunctionName("UpdateBook")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "books/{bookId}")] HttpRequest req,
            ILogger log, int bookId, CancellationToken token)
        {
            var auth = await AuthenticateAsWriter(req, log);
            var book = await ReadBody<BookView>(req);
            book.Id = bookId;

            var request = new UpdateBookRequest(book.Map());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _bookRenderer.Render(auth.User, request.Result.Book);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }
            else
            {
                return new OkObjectResult(renderResult);
            }
        }

        public static LinkView Link(int bookId, string relType = RelTypes.Self) => SelfLink($"books/{bookId}", relType, "PUT");
    }
}
