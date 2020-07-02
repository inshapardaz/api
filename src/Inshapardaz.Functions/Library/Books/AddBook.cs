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

namespace Inshapardaz.Functions.Library.Books
{
    public class AddBook : CommandBase
    {
        public AddBook(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("AddBook")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "library/{libraryId}/books")]
            BookView book,
            int libraryId,
            ClaimsPrincipal claims,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var request = new AddBookRequest(claims, libraryId, book.Map());
                await CommandProcessor.SendAsync(request, cancellationToken: token);

                var renderResult = request.Result.Render(claims);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            });
        }

        public static LinkView Link(int libraryId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/books", relType, "POST");
    }
}
