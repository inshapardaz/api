using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books
{
    public class UpdateBook : FunctionBase
    {
        public UpdateBook(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("UpdateBook")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "books/{bookId}")] BookView book,
            ILogger log, int bookId, [AccessToken] ClaimsPrincipal principal, CancellationToken token)
        {
            if (principal == null)
            {
                return new UnauthorizedResult();
            }

            if (!principal.IsWriter())
            {
                return new ForbidResult("Bearer");
            }

            book.Id = bookId;

            var request = new UpdateBookRequest(book.Map());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = request.Result.Book.Render(principal);

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