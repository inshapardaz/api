using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Inshapardaz.Functions.Library;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions
{
    public class GetLanguages : FunctionBase
    {
        [FunctionName("GetLanguages")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "languages")] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult("GET: Languages");
        }

        public static LinkView Self(string relType = RelTypes.Self) => SelfLink("/languages", relType);
    }
}
