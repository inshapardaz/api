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

namespace Inshapardaz.Functions.Library.Authors
{
    public class GetAuthorById : QueryBase
    {
        public GetAuthorById(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetAuthorById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/authors/{authorId:int}")] HttpRequest req,
            int libraryId, int authorId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            var query = new GetAuthorByIdQuery(libraryId, authorId);
            var author = await QueryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (author != null)
            {
                return new OkObjectResult(author.Render(principal));
            }

            return new NotFoundResult();
        }

        public static LinkView Link(int authorId, string relType = RelTypes.Self) => SelfLink($"authors/{authorId}", relType, "GET");
    }
}
