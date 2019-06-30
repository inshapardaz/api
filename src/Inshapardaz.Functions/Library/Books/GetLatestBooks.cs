using System.Threading.Tasks;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books
{
    public class GetLatestBooks : FunctionBase
    {
        public GetLatestBooks(IAmACommandProcessor commandProcessor) 
        : base(commandProcessor)
        {
        }

        [FunctionName("GetLatestBooks")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books/latest")] HttpRequest req,
            ILogger log)
        {
            // pageSize
            return new OkObjectResult("GET:Latest Books");
        }

        public static LinkView Self(string relType = RelTypes.Self) => SelfLink("books/latest", relType);
    }
}
