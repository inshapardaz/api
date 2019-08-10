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

namespace Inshapardaz.Functions.Library.Books
{
    public class UpdateBookImage : FunctionBase
    {
        public UpdateBookImage(IAmACommandProcessor commandProcessor) 
        : base(commandProcessor)
        {
        }

        [FunctionName("UpdateBookImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "books/{bookId}/image")] HttpRequestMessage req,
            int bookId,
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

            var fileName = $"{bookId}";
            var mimeType = "application/binary";
            var fileContent = multipart.Contents.FirstOrDefault();
            if (fileContent != null)
            {
                fileName = $"{bookId}{GetFileExtension(fileContent.Headers?.ContentDisposition?.FileName)}";
                mimeType = fileContent.Headers?.ContentType?.MediaType;
            }

            var request = new UpdateBookImageRequest(bookId)
            {
                Image = new Domain.Entities.File
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

        public static LinkView Link(int bookId, string relType = RelTypes.Self) => SelfLink($"books/{bookId}/image", relType, "PUT");

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
