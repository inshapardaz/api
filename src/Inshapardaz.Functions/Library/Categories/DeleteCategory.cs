using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Categories
{
    public class DeleteCategory : FunctionBase
    {
        public DeleteCategory(IAmACommandProcessor commandProcessor) 
        : base(commandProcessor)
        {
        }

        [FunctionName("DeleteCategory")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "categories/{id}")] HttpRequestMessage req,
            ILogger log, int id, CancellationToken token)
        {
            var auth = await AuthenticateAsWriter(req, log);

             var request = new DeleteCategoryRequest(id);
            await CommandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
