using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Authentication;
using System.Security.Claims;
using System.Threading;
using Inshapardaz.Domain.Ports.Dictionaries;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Dictionaries
{
    public class DeleteDictionary : CommandBase
    {
        public DeleteDictionary(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("DeleteDictionary")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "dictionaries/{dictionaryId:int}")] HttpRequest req,
            int dictionaryId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            if (principal == null)
            {
                return new UnauthorizedResult();
            }

            if (!principal.IsWriter())
            {
                return new ForbidResult("Bearer");
            }

            var request = new DeleteDictionaryRequest(dictionaryId);
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            return new OkResult();
        }

        public static LinkView Link(int dictionaryId, string relType = RelTypes.Self) => SelfLink($"dictionaries/{dictionaryId}", relType, "DELETE");
    }
}
