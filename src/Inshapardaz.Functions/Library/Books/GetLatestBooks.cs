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
using Paramore.Darker;

namespace Inshapardaz.Functions.Library.Books
{
    public class GetLatestBooks : QueryBase
    {
        public GetLatestBooks(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetLatestBooks")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/books/latest")] HttpRequest req,
            int libraryId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            var pageSize = GetQueryParameter(req, "pageSize", 10);

            var query = new GetLatestBooksQuery(principal.GetUserId());
            var books = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            return new OkObjectResult(books.Render(principal, Link));
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("books/latest", relType);
    }
}
