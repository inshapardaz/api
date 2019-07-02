using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Adapters.Library;
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
    public class GetCategoryById : FunctionBase
    {
        private readonly IRenderCategory _categoryRenderer;

        public GetCategoryById(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator, IRenderCategory categoryRenderer)
        : base(commandProcessor, authenticator)
        {
            _categoryRenderer = categoryRenderer;
        }

        [FunctionName("GetCategoryById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "categories/{categoryById}")] HttpRequest req,
            ILogger log, int categoryById, CancellationToken token)
        {
            var auth = await TryAuthenticate(req, log);

            var request = new GetCategoryByIdRequest(categoryById);
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            return new OkObjectResult(_categoryRenderer.Render(auth?.User, request.Result));
        }

        public static LinkView Self(int categoryById, string relType = RelTypes.Self) => SelfLink($"categories/{categoryById}", relType);
    }
}
