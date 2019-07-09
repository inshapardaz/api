using System.Linq;
using System.Net.Http;
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
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Authors
{
    public class UpdateAuthorImage : FunctionBase
    {
        public UpdateAuthorImage(IAmACommandProcessor commandProcessor) 
        : base(commandProcessor)
        {
        }

        [FunctionName("UpdateAuthorImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "authors/{id}/image")] HttpRequestMessage req,
            ILogger log, int id,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            var provider = new MultipartMemoryStreamProvider();
            await req.Content.ReadAsMultipartAsync(provider, token);
            var file = provider.Contents.First();
            var fileInfo = file.Headers.ContentDisposition;
            var fileData = await file.ReadAsByteArrayAsync();

            var request = new UpdateAuthorImageRequest(id)
            {
                Image = new Domain.Entities.File
                {
                    FileName = fileInfo.FileName,
                    MimeType = file.Headers.ContentType.MediaType,
                    Contents = fileData
                }
            };

            await CommandProcessor.SendAsync(request, cancellationToken: token);

            //if (request.Result.HasAddedNew)
            //{
            //    var response = _fileRenderer.Render(request.Result.File);
            //    return new CreatedResult(request.re.Links.Self(), response);
            //}

            return new OkObjectResult($"PUT:Author {id} Image");
        }

        public static LinkView Link(int authorId, string relType = RelTypes.Self) => SelfLink($"authors/{authorId}/image", relType, "PUT");
    }
}
