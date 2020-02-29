using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Authors
{
    public class UpdateAuthorImage : CommandBase
    {
        public UpdateAuthorImage(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("UpdateAuthorImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "library/{libraryId}/authors/{id:int}/image")]
            HttpRequestMessage req,
            int libraryId, int authorId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token = default)
        {
            if (principal == null)
            {
                return new UnauthorizedResult();
            }

            if (!principal.IsWriter())
            {
                return new ForbidResult("Bearer");
            }

            var multipart = await req.Content.ReadAsMultipartAsync(token);
            var content = await req.Content.ReadAsByteArrayAsync();

            var fileName = $"{authorId}";
            var mimeType = "application/binary";
            var fileContent = multipart.Contents.FirstOrDefault();
            if (fileContent != null)
            {
                fileName = $"{authorId}{GetFileExtension(fileContent.Headers?.ContentDisposition?.FileName)}";
                mimeType = fileContent.Headers?.ContentType?.MediaType;
            }

            var request = new UpdateAuthorImageRequest(libraryId, authorId)
            {
                Image = new Domain.Models.FileModel
                {
                    FileName = fileName,
                    MimeType = mimeType,
                    Contents = content
                }
            };

            await CommandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result.HasAddedNew)
            {
                var response = request.Result.File.Render(principal);
                return new CreatedResult(response.Links.Self(), response);
            }

            return new OkResult();
        }

        public static LinkView Link(int authorId, string relType = RelTypes.Self) => SelfLink($"authors/{authorId}/image", relType, "PUT");

        private string GetFileExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return System.IO.Path.GetExtension(fileName);
            }

            return "";
        }
    }
}
