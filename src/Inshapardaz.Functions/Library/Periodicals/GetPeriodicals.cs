using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Periodicals
{
    public class GetPeriodicals : CommandBase
    {
        public GetPeriodicals(IAmACommandProcessor commandProcessor) 
        : base(commandProcessor)
        {
        }

        [FunctionName("GetPeriodicals")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "periodicals")] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult("GET:Periodicals");
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("periodicals", relType);
    }
}
