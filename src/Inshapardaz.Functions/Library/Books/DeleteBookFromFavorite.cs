using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
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
    public class DeleteBookFromFavorite : CommandBase
    {
        public DeleteBookFromFavorite(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("DeleteBookFromFavorite")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "library/{libraryId}/books/favorite/{bookId:int}")] HttpRequest req,
            int libraryId, int bookId,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token)
        {
            var request = new DeleteBookToFavoriteRequest(claims, libraryId, bookId, claims.GetUserId());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            return new CreatedResult(new Uri(GetFavoriteBooks.Link(RelTypes.Self).Href), null);
        }

        public static LinkView Link(int bookId, string relType = RelTypes.Self) => SelfLink($"books/favorite/{bookId}", relType, "Delete");
    }
}
