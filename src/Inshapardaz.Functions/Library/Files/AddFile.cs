using Inshapardaz.Domain.Models;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Inshapardaz.Functions.Library.Files
{
    public class AddFile : CommandBase
    {
        public AddFile(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("AddFile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "files")]
            HttpRequestMessage req,
            ILogger log,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token)
        {
            var multipart = await req.Content.ReadAsMultipartAsync(token);
            var content = await req.Content.ReadAsByteArrayAsync();

            var fileName = Guid.NewGuid().ToString("N");
            var mimeType = "application/binary";
            var fileContent = multipart.Contents.FirstOrDefault();
            if (fileContent != null)
            {
                fileName = fileContent.Headers?.ContentDisposition?.FileName ?? Guid.NewGuid().ToString("N");
                mimeType = fileContent.Headers?.ContentType.MediaType;
            }

            var request = new AddFileRequest(new Domain.Models.FileModel
            {
                FileName = fileName,
                MimeType = mimeType,
                Contents = content
            });

            await CommandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Response == null)
            {
                return new InternalServerErrorResult();
            }

            var response = request.Response.Render(claims);
            return new CreatedResult(response.Links.Self(), response);
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("files", relType, "POST");
    }
}
