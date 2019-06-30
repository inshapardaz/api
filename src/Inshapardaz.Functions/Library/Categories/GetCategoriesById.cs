using System.Threading.Tasks;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Categories
{
    public class GetCategoryById : FunctionBase
    {
        public GetCategoryById(IAmACommandProcessor commandProcessor) 
        : base(commandProcessor)
        {
        }

        [FunctionName("GetCategoryById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "categories/{categoryById}")] HttpRequest req,
            ILogger log, int categoryById)
        {
            return new OkObjectResult($"Get:Category {categoryById}");
        }

        public static LinkView Self(int categoryById, string relType = RelTypes.Self) => SelfLink($"categories/{categoryById}", relType);
    }
}
