using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Adapters.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Extentions;
using Inshapardaz.Functions.View.Library;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Categories
{
    public class AddCategory : FunctionBase
    {
        private readonly IRenderCategory _renderCategory;

        public AddCategory(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator, IRenderCategory renderCategory)
        : base(commandProcessor, authenticator)
        {
            _renderCategory = renderCategory;
        }

        [FunctionName("AddCategory")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "categories")] HttpRequestMessage req,
            ILogger log, CancellationToken token)
        {
            var auth = await AuthenticateAsWriter(req, log);
            var category = await ReadBody<CategoryView>(req);

            var request = new AddCategoryRequest(category.Map<CategoryView, Category>());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _renderCategory.Render(auth.User, request.Result);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("categories", relType, "POST");
    }
}
