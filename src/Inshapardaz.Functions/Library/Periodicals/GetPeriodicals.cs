using System.Threading.Tasks;
using Inshapardaz.Functions.Extentions;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Periodicals
{
    public class GetPeriodicals : FunctionBase
    {
        [FunctionName("GetPeriodicals")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "periodicals")] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult("GET:Periodicals");
        }

        public static LinkView Self(string relType = RelTypes.Self) => SelfLink("/periodicals", relType);
    }
}
