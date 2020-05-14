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

namespace Inshapardaz.Functions.Library.Series
{
    public class UpdateSeriesImage : CommandBase
    {
        public UpdateSeriesImage(IAmACommandProcessor commandProcessor)
        : base(commandProcessor)
        {
        }

        [FunctionName("UpdateSeriesImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "library/{libraryId}/series/{seriesId:int}/image")]
            HttpRequestMessage req,
            int libraryId,
            int seriesId,
            // TODO: Make this work
            //[FromFile]  object file,
            [AccessToken] ClaimsPrincipal claims,
            CancellationToken token = default)
        {
            return await Executor.Execute(async () =>
            {
                var multipart = await req.Content.ReadAsMultipartAsync(token);
                var content = await req.Content.ReadAsByteArrayAsync();

                var fileName = $"{seriesId}";
                var mimeType = "application/binary";
                var fileContent = multipart.Contents.FirstOrDefault();
                if (fileContent != null)
                {
                    fileName = $"{seriesId}{GetFileExtension(fileContent.Headers?.ContentDisposition?.FileName)}";
                    mimeType = fileContent.Headers?.ContentType?.MediaType;
                }

                var request = new UpdateSeriesImageRequest(claims, libraryId, seriesId)
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

        public static LinkView Link(int libraryId, int seriesId, string relType = RelTypes.Self)
            => SelfLink($"library/{libraryId}/series/{seriesId}/image", relType, "PUT");

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
