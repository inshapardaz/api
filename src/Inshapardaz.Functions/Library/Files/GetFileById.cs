using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Files
{
    public class GetFileById : CommandBase
    {
        public GetFileById(IAmACommandProcessor commandProcessor) 
        : base(commandProcessor)
        {
        }

        [FunctionName("GetFileById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "files/{fileId:int}")] HttpRequest req,
            ILogger log, int fileId,
            [AccessToken] ClaimsPrincipal claims, 
            CancellationToken token)
        {
            // TODO : Secure private files
            var request = new GetFileRequest(fileId, 200, 200);
            await CommandProcessor.SendAsync(request, true, token);

            if (request.Response == null)
            {
                return new NotFoundResult();
            }

            return new FileContentResult(request.Response.Contents, new MediaTypeHeaderValue(request.Response.MimeType));
}

        public static LinkView Link(int fileId, string relType = RelTypes.Self) => SelfLink($"files/{fileId}", relType, "GET");        
    }
}
