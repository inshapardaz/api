using System.Threading.Tasks;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books
{
    public class GetBooksBySeries : FunctionBase
    {
        public GetBooksBySeries(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator) 
        : base(commandProcessor, authenticator)
        {
        }

        [FunctionName("GetBooksBySeries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series/{id}/books")] HttpRequest req,
            ILogger log, int id)
        {
            // parameters
            // query
            // pageNumber
            // pageSize
            // orderBy
            return new OkObjectResult($"GET:Books for Series {id}");
        }

        public static LinkView Link(int seriesId, string relType = RelTypes.Self) => SelfLink($"series/{seriesId}/books", relType);
    }
}
