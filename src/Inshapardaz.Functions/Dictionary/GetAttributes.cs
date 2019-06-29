using System.Threading.Tasks;
using Inshapardaz.Functions.Library;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Dictionary
{
    public class GetAttributes : FunctionBase
    {
        [FunctionName("GetAttributes")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "attributes")] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult("GET:Attributes");
        }

        public static LinkView Self(string relType = RelTypes.Self) => SelfLink("/attributes", relType);
    }
}
