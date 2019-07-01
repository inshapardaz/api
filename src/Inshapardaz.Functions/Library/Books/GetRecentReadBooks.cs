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
    public class GetRecentReadBooks : FunctionBase
    {
        public GetRecentReadBooks(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator) 
        : base(commandProcessor, authenticator)
        {
        }

        [FunctionName("GetRecentReadBooks")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books/recent")] HttpRequest req,
            ILogger log)
        {
            // pageSize
            return new OkObjectResult("GET:Recent Books");
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("books/recent", relType);

    }
}
