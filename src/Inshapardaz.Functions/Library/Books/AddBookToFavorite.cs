using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "books/favorite/{id:int}")] HttpRequest req, 
            int id,
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

            var request = new AddBookToFavoriteRequest(id, principal.GetUserId());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            return new CreatedResult(new Uri(GetFavoriteBooks.Link(RelTypes.Self).Href), null);
        }

        public static LinkView Link(int bookId, string relType = RelTypes.Self) => SelfLink($"books/favorite/{bookId}", relType, "POST");
    }
}
