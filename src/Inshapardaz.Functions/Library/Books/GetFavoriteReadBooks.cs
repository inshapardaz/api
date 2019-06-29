using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Books
{
    public static class GetFavoriteReadBooks
    {
        [FunctionName("GetFavoriteReadBooks")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "books/favorite")] HttpRequest req,
            ILogger log)
        {
            // pageSize
            return new OkObjectResult("GET:Favorite Books");
        }
    }
}
