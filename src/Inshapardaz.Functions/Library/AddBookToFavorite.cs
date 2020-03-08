using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "library/{libraryId}/favorites")] AddToFavoriteView view,
            int libraryId,
            int bookId,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var request = new AddBookToFavoriteRequest(claims, libraryId, view.BookId, claims.GetUserId());
                await CommandProcessor.SendAsync(request, cancellationToken: token);

                return new CreatedResult(new Uri(GetFavoriteBooks.Link(libraryId, RelTypes.Self).Href), null);
            });
        }

        public static LinkView Link(int libraryId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/favorites", relType, "POST");
    }
}
