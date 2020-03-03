using Inshapardaz.Domain.Ports;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Paramore.Darker;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Files
{
    public class GetFileById : QueryBase
    {
        public GetFileById(IQueryProcessor queryProcessor)
        : base(queryProcessor)
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
            var query = new GetFileQuery(fileId, 200, 200);
            var file = await QueryProcessor.ExecuteAsync(query, token);

            if (file == null)
            {
                return new NotFoundResult();
            }

            return new FileContentResult(file.Contents, new MediaTypeHeaderValue(file.MimeType));
        }

        public static LinkView Link(int fileId, string relType = RelTypes.Self) => SelfLink($"files/{fileId}", relType, "GET");
    }
}
