using Inshapardaz.Domain.Models.Handlers;
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

namespace Inshapardaz.Functions
{
    public class GetLibrary : QueryBase
    {
        public GetLibrary(IQueryProcessor queryProcessor)

            : base(queryProcessor)
        {
        }

        [FunctionName("GetLibrary")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}")] HttpRequest req,
                                             int libraryId,
                                             ClaimsPrincipal principal,
                                             CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = new GetLibraryQuery(libraryId, principal);
            var library = await QueryProcessor.ExecuteAsync(query, cancellationToken);
            return new OkObjectResult(library.Render(principal));
        }

        public static LinkView Link(int libraryId, string relType = RelTypes.Self) => SelfLink($"/library/{libraryId}", relType);
    }
}
