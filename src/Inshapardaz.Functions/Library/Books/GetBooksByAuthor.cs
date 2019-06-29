using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Library.Books
{
    public static class GetBooksByAuthor
    {
        [FunctionName("GetBooksByAuthor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authors/{id}/books")] HttpRequest req,
            ILogger log, int id)
        {
            // parameters
            // query
            // pageNumber
            // pageSize
            // orderBy
            return new OkObjectResult($"GET:Books for Author {id}");
        }
    }
}
