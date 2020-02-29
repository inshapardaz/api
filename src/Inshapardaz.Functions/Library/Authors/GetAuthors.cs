using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
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

namespace Inshapardaz.Functions.Library.Authors
{
    public class GetAuthors : QueryBase
    {
        public GetAuthors(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetAuthors")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/authors")] HttpRequest req,
            int libraryId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            var query = GetQueryParameter<string>(req, "query", null);
            var pageNumber = GetQueryParameter(req, "pageNumber", 1);
            var pageSize = GetQueryParameter(req, "pageSize", 10);

            var authorsQuery = new GetAuthorsQuery(libraryId, pageNumber, pageSize) { Query = query };
            var result = await QueryProcessor.ExecuteAsync(authorsQuery, token);

            var args = new PageRendererArgs<AuthorModel>
            {
                Page = result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                LinkFunc = Link
            };

            return new OkObjectResult(args.Render(principal));
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("authors", relType);

        public static LinkView Link(int pageNumber = 1, int pageSize = 10, string relType = RelTypes.Self)
            => SelfLink("authors", relType, queryString: new Dictionary<string, string>
            {
                { "pageNumber", pageNumber.ToString()},
                { "pageSize", pageSize.ToString()}
            });
    }
}
