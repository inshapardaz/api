using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Books
{
    public class DeleteBookFromFavorite : FunctionBase
    {
        public DeleteBookFromFavorite(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("DeleteBookFromFavorite")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "books/favorite/{id}")] int id,
            [AccessToken] ClaimsPrincipal principal, 
            CancellationToken token)
        {
            if (principal == null)
            {
                return new UnauthorizedResult();
            }

            if (!principal.IsAuthenticated())
            {
                return new ForbidResult("Bearer");
            }

            var request = new DeleteBookToFavoriteRequest(id, principal.GetUserId());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            return new CreatedResult(new Uri(GetFavoriteBooks.Link(RelTypes.Self).Href), null);
        }

        public static LinkView Link(int bookId, string relType = RelTypes.Self) => SelfLink($"books/favorite/{bookId}", relType, "Delete");
    }
}
