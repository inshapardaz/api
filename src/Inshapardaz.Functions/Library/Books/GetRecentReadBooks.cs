using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Darker;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Books
{
    public class GetRecentReadBooks : QueryBase
    {
        public GetRecentReadBooks(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetRecentReadBooks")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/recents")] HttpRequest req,
            int libraryId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            if (principal == null || !principal.IsAuthenticated())
            {
                return new UnauthorizedResult();
            }

            var pageSize = GetQueryParameter(req, "pageSize", 10);

            var query = new GetRecentBooksQuery(libraryId, principal.GetUserId().Value, pageSize);
            var books = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            return new OkObjectResult(books.Render(principal, Link));
        }

        public static LinkView Link(int libraryId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/recents", relType);
    }
}
