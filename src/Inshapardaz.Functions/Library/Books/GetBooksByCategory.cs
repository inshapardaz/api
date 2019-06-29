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
    public class GetBooksByCategory : FunctionBase
    {
        public GetBooksByCategory(IAmACommandProcessor commandProcessor) 
        : base(commandProcessor)
        {
        }

        [FunctionName("GetBooksByCategory")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "categories/{categoryId}/books")] HttpRequest req,
            ILogger log, int categoryId)
        {
            // parameters
            // query
            // pageNumber
            // pageSize
            // orderBy
            return new OkObjectResult($"GET:Books for Category {categoryId}");
        }

        public static LinkView Self(int categoryById, string relType = RelTypes.Self) => SelfLink($"/categories/{categoryById}/books", relType);

    }
}
