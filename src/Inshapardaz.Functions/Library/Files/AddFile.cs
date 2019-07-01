using System.Threading.Tasks;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Files
{
    public class AddFile : FunctionBase
    {
        public AddFile(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator) 
        : base(commandProcessor, authenticator)
        {
        }

        [FunctionName("AddFile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "files")] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult("POST:Files");
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("files", relType, "POST");
    }
}
