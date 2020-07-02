using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Books
{
    public class AddBookToFavorite : CommandBase
    {
        public AddBookToFavorite(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("AddBookToFavorite")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "library/{libraryId}/favorites/books/{bookId}")] HttpRequest req,
            int libraryId,
            int bookId,
            ClaimsPrincipal claims,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var request = new AddBookToFavoriteRequest(claims, libraryId, bookId, claims.GetUserId());
                await CommandProcessor.SendAsync(request, cancellationToken: token);

                return new CreatedResult(new Uri(Link(libraryId, bookId, RelTypes.Self).Href), null);
            });
        }

        public static LinkView Link(int libraryId, int bookId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/favorites/books/{bookId}", relType, "POST");
    }
}
