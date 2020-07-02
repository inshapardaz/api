using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "library/{libraryId}/categories/{categoryId:int}")]
            CategoryView category,
            int libraryId, int categoryId,
            ClaimsPrincipal claims,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                category.Id = categoryId;
                var request = new UpdateCategoryRequest(claims, libraryId, category.Map());
                await CommandProcessor.SendAsync(request, cancellationToken: token);

                var renderResult = request.Result.Category.Render(libraryId, claims);

                if (request.Result.HasAddedNew)
                {
                    return new CreatedResult(renderResult.Links.Self(), renderResult);
                }
                else
                {
                    return new OkObjectResult(renderResult);
                }
            });
        }

        public static LinkView Link(int libraryId, int categoryId, string relType = RelTypes.Self)
            => SelfLink($"library/{libraryId}/categories/{categoryId}", relType, "PUT");
    }
}
