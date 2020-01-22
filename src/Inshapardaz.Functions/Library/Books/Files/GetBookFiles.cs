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
using Paramore.Darker;

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books/{bookId}/files")] HttpRequest req,
            int bookId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            var query = new GetFilesByBookQuery(bookId, principal.GetUserId());
            var files = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (files != null)
            {
                return new OkObjectResult(files.Render(principal));
            }

            return new NotFoundResult();
        }

        public static LinkView Link(int bookId, string relType = RelTypes.Self) => SelfLink($"books/{bookId}/files", relType);        
    }
}
