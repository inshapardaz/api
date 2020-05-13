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

namespace Inshapardaz.Functions.Library.Books.Files
{
    public class GetBookFiles : QueryBase
    {
        public GetBookFiles(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetBookFiles")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/books/{bookId}/files")] HttpRequest req,
            int libraryId,
            int bookId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            var query = new GetFilesByBookQuery(libraryId, bookId, principal.GetUserId());
            var files = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (files != null)
            {
                return new OkObjectResult(files.Render(principal));
            }

            return new NotFoundResult();
        }

        public static LinkView Link(int libraryId, int bookId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/books/{bookId}/files", relType);
    }
}
