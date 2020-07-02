using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Converters;
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
using Executor = Inshapardaz.Functions.Extensions.Executor;

namespace Inshapardaz.Functions.Library.Books.Content
{
    public class UpdateBookContent : CommandBase
    {
        public UpdateBookContent(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("UpdateBookFile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "library/{libraryId}/books/{bookId}/content")] HttpRequestMessage req,
            int libraryId,
            int bookId,
            ClaimsPrincipal claims,
            CancellationToken token)
        {
            return await Executor.Execute(async () =>
            {
                var multipart = await req.Content.ReadAsMultipartAsync(token);
                var content = await req.Content.ReadAsByteArrayAsync();

                var fileName = $"{bookId}";

                var mimeType = GetHeader(req, "Content-Type", "text/markdown");
                var language = GetHeader(req, "Accept-Language", "");

                var fileContent = multipart.Contents.FirstOrDefault();
                if (fileContent != null)
                {
                    fileName = $"{bookId}{GetFileExtension(fileContent.Headers?.ContentDisposition?.FileName)}";
                    mimeType = fileContent.Headers?.ContentType?.MediaType;
                }

                var request = new UpdateBookContentRequest(claims, libraryId, bookId, language, mimeType)
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

                if (request.Result.Content != null)
                {
                    var renderResult = request.Result.Content.Render(libraryId, claims);

                    if (request.Result.HasAddedNew)
                    {
                        return new CreatedResult(renderResult.Links.Self(), renderResult);
                    }
                    else
                    {
                        return new OkObjectResult(renderResult);
                    }
                }

                return new BadRequestResult();
            });
        }

        private object GetFileExtension(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                return System.IO.Path.GetExtension(fileName);
            }

            return "";
        }

        public static LinkView Link(int libraryId, int bookId, string mimetype, string langugae, string relType = RelTypes.Self)
            => SelfLink($"library/{libraryId}/books/{bookId}/content", relType, "PUT", media: mimetype, language: langugae);
    }
}
