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
    public class AddBook : FunctionBase
    {
        private readonly IRenderBook _bookRenderer;
        public AddBook(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator, IRenderBook bookRenderer)
        : base(commandProcessor, authenticator)
        {
            _bookRenderer = bookRenderer;
        }

        [FunctionName("AddBook")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "books")] HttpRequest req,
            ILogger log, CancellationToken token)
        {
            var auth = await AuthenticateAsWriter(req, log);
            var category = await ReadBody<BookView>(req);

            var request = new AddBookRequest(category.Map());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _bookRenderer.Render(auth.User, request.Result);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("books", relType, "POST");
    }
}
