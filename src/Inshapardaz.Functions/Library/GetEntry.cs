using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Commands;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Inshapardaz.Functions
{
    public class GetEntry : CommandBase
    {
        public GetEntry(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("GetEntry")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "/library/{libraryId}")] HttpRequest req,
                                             int libraryId,
                                             ClaimsPrincipal principal)
        {
            if (principal != null && principal.IsAuthenticated())
            {
                log.LogInformation("User authenticated");
                foreach (var claim in principal.Claims)
                    log.LogInformation($"Claim `{claim.Type}` is `{claim.Value}`");
            }
            else
            {
                log.LogWarning("User is not authenticated");
            }

            var command = new GetEntryRequest(principal);
            await CommandProcessor.SendAsync(command);
            return new OkObjectResult(command.Result);
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("entry", relType);
    }
}
