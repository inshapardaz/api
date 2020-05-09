using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Authors
{
    public class DeleteAuthor : CommandBase
    {
        public DeleteAuthor(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("DeleteAuthor")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "library/{libraryId}/authors/{authorId:int}")] HttpRequest req,
            int libraryId, int authorId,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var request = new DeleteAuthorRequest(claims, libraryId, authorId);
                await CommandProcessor.SendAsync(request, cancellationToken: token);
                return new NoContentResult();
            });
        }

        public static LinkView Link(int libraryId, int authorId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/authors/{authorId}", relType, "DELETE");
    }
}
