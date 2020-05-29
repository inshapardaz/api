using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Brighter;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Books.Content
{
    public class AddBookContent : CommandBase
    {
        public AddBookContent(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("AddBookFile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "library/{libraryId}/books/{bookId}/content")] HttpRequestMessage req,
            int libraryId,
            int bookId,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var multipart = await req.Content.ReadAsMultipartAsync(token);
                var content = await req.Content.ReadAsByteArrayAsync();

                var mimeType = GetHeader(req, "Content-Type", "text/markdown");
                var language = GetHeader(req, "Accept-Language", "");

                var fileName = $"{bookId}";
                var fileContent = multipart.Contents.FirstOrDefault();
                if (fileContent != null)
                {
                    fileName = $"{bookId}{GetFileExtension(fileContent.Headers?.ContentDisposition?.FileName)}";
                    mimeType = fileContent.Headers?.ContentType?.MediaType;
                }

                var request = new AddBookContentRequest(claims, libraryId, bookId, language, mimeType)
                {
                    Content = new FileModel
                    {
                        Contents = content,
                        MimeType = mimeType,
                        DateCreated = DateTime.Now,
                        FileName = fileName
                    }
                };

                await CommandProcessor.SendAsync(request, cancellationToken: token);

                if (request.Result != null)
                {
                    var response = request.Result.Render(libraryId, claims);
                    return new CreatedResult(response.Links.Self(), response);
                }

                return new BadRequestResult();
            });
        }

        public static LinkView Link(int libraryId, int bookId, string relType = RelTypes.Self)
            => SelfLink($"library/{libraryId}/books/{bookId}/content", relType, "POST");

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
