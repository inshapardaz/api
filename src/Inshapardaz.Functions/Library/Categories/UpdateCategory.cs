using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Adapters.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Categories
{
    public class UpdateCategory : FunctionBase
    {
        private readonly IRenderCategory _categoryRenderer;

        public UpdateCategory(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator, IRenderCategory categoryRenderer)
        : base(commandProcessor, authenticator)
        {
            _categoryRenderer = categoryRenderer;
        }

        [FunctionName("UpdateCategory")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "categories/{id}")] HttpRequest req,
            ILogger log, int id, CancellationToken token)
        {
            var auth = await AuthenticateAsWriter(req, log);
            var category = await ReadBody<CategoryView>(req);

            var request = new UpdateCategoryRequest(category.Map<CategoryView, Category>());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _categoryRenderer.Render(auth.User, request.Result.Category);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }
            else
            {
                return new OkObjectResult(renderResult);
            }
        }

        public static LinkView Link(int categoryId, string relType = RelTypes.Self) => SelfLink($"categories/{categoryId}", relType, "PUT");
    }
}
