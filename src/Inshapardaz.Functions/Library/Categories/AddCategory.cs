using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Categories
{
    public class AddCategory : FunctionBase
    {
        public AddCategory(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("AddCategory")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "categories")] CategoryView category,
            ILogger log, [AccessToken] ClaimsPrincipal principal, CancellationToken token)
        {
            if (principal == null)
            {
                return new UnauthorizedResult();
            }

            if (!principal.IsAdministrator())
            {
                return new ForbidResult("Bearer");
            }

            var request = new AddCategoryRequest(category.Map());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = request.Result.Render(principal);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("categories", relType, "POST");
    }
}