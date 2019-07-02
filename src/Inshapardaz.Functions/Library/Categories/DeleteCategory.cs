using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Categories
{
    public class DeleteCategory : FunctionBase
    {
        public DeleteCategory(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator) 
        : base(commandProcessor, authenticator)
        {
        }

        [FunctionName("DeleteCategory")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "categories/{id}")] HttpRequest req,
            ILogger log, int id, CancellationToken token)
        {
            var auth = await AuthenticateAsWriter(req, log);

             var request = new DeleteCategoryRequest(id);
            await CommandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        public static LinkView Link(int categoryId, string relType = RelTypes.Self) => SelfLink($"categories/{categoryId}", relType, "DELETE");
    }
}
