using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
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

namespace Inshapardaz.Functions.Library.Books.Files
{
    public class UpdateBookFile : CommandBase
    {
        public UpdateBookFile(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("UpdateBookFile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "library/{libraryId}/books/{bookId}/files/{fileId}")] HttpRequestMessage req,
            int libraryId,
            int bookId,
            int fileId,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token)
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

                var request = new UpdateBookFileRequest(claims, libraryId, bookId, fileId)
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
                    var renderResult = request.Result.Content.Render(claims);

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
    }
}
