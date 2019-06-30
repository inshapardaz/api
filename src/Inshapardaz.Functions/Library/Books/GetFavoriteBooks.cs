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
    public class GetFavoriteBooks : FunctionBase
    {
        public GetFavoriteBooks(IAmACommandProcessor commandProcessor) 
        : base(commandProcessor)
        {
        }

        [FunctionName("GetFavoriteBooks")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books/favorite")] HttpRequest req,
            ILogger log)
        {
            // pageSize
            return new OkObjectResult("GET:Favorite Books");
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("books/favorite", relType);
    }
}
