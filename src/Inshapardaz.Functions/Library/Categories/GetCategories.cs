using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Adapters.Library;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Categories
{
    public class GetCategories : FunctionBase
    {
        private readonly IRenderCategories _categoriesRenderer;
        public GetCategories(IAmACommandProcessor commandProcessor, IRenderCategories categoriesRenderer)
        : base(commandProcessor)
        {
            _categoriesRenderer = categoriesRenderer;
        }

        [FunctionName("GetCategories")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "categories")] HttpRequestMessage req,
            ILogger log, CancellationToken token)
        {
            var auth = await TryAuthenticate(req, log);

            var request = new GetCategoriesRequest();
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            return new OkObjectResult(_categoriesRenderer.Render(request.Result));
        }

        public static LinkView Self(string relType = RelTypes.Self) => SelfLink("/categories", relType);
    }
}
