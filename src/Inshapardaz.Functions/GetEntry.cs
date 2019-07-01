using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Commands;
using System.Net.Http;
using Inshapardaz.Functions.Authentication;

namespace Inshapardaz.Functions
{
    public class GetEntry : FunctionBase
    {
        public GetEntry(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator)
            : base(commandProcessor, authenticator)
        {   
        }

        [FunctionName("GetEntry")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "entry")] HttpRequestMessage req, ILogger log)
        {
            var auth = await TryAuthenticate(req, log);
            
            if (auth.HasValue)
            {
                log.LogInformation("User authenticated");
                foreach (var claim in auth.Value.User.Claims)
                    log.LogInformation($"Claim `{claim.Type}` is `{claim.Value}`");
            }

            var command = new GetEntryRequest(auth?.User);
            await CommandProcessor.SendAsync(command);
            return new OkObjectResult(command.Result);
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("entry", relType);
    }
}
