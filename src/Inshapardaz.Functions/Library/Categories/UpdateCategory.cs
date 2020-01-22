using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Categories
{
    public class UpdateCategory : CommandBase
    {
        public UpdateCategory(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("UpdateCategory")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "categories/{categoryId:int}")] HttpRequest req,
            int categoryId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            if (principal == null)
            {
                return new UnauthorizedResult();
            }

            if (!principal.IsAdministrator())
            {
                return new ForbidResult("Bearer");
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var category = JsonConvert.DeserializeObject<CategoryView>(requestBody);

            category.Id = categoryId;
            var request = new UpdateCategoryRequest(category.Map());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = request.Result.Category.Render(principal);

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
