using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Inshapardaz.Api.Helpers
{
    public static class FileExtensions
    {
        public static async Task<byte[]> ReadContentsAsync(this IFormFile file, CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return content;
        }
    }
}
