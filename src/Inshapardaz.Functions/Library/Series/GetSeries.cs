using System.Threading.Tasks;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Series
{
    public class GetSeries : FunctionBase
    {
        [FunctionName("GetSeries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "series")] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult("GET:Series");
        }

        public static LinkView Self(string relType = RelTypes.Self) => SelfLink("/series", relType);
    }
}
