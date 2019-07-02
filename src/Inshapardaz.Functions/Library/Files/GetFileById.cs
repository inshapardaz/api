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
    public class GetFileById : FunctionBase
    {
        public GetFileById(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator) 
        : base(commandProcessor, authenticator)
        {
        }

        [FunctionName("GetFileById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "files/{fileId}.{extention?}")] HttpRequest req,
            ILogger log, int fileId, string extention)
        {
            // parameters 
            // height = 200
            // width = 200, 
            return new OkObjectResult($"Get:Series {fileId}.{extention}");
        }

        public static LinkView Link(int fileId, string relType = RelTypes.Self) => SelfLink($"files/{fileId}", relType, "GET");        
    }
}
