using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Books
{
    public class UpdateBookImage : CommandBase
    {
        public UpdateBookImage(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("UpdateBookImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "library/{libraryId}/books/{bookId:int}/image")] HttpRequestMessage req,
            int libraryId,
            int bookId,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token = default)
        {
            return await Executor.Execute(async () =>
            {
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

                var request = new UpdateBookImageRequest(claims, libraryId, bookId)
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
                    var response = request.Result.File.Render(claims);

                    return new CreatedResult(response.Links.Self(), response);
                }

                return new OkResult();
            });
        }

        public static LinkView Link(int libraryId, int bookId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/books/{bookId}/image", relType, "PUT");

        private string GetFileExtension(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                return System.IO.Path.GetExtension(fileName);
            }

            return "";
        }
    }
}
