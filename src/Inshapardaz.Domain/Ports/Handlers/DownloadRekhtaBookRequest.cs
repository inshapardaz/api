using Common;
using Paramore.Brighter;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models
{
    public class DownloadRekhtaBookRequest : RequestBase
    {
        public DownloadRekhtaBookRequest(string url)
        {
            Url = url;
        }

        public string Url { get; init; }

        public bool CreatePdf { get; set; }

        public Result DownloadResult { get; set; }

        public class Result
        {
            public byte[] File { get; set; }

            public string FileName { get; set; }

            public string MimeType { get; set; }
        }
    }

    public class DownloadRekhtaBookRequestHandler : RequestHandlerAsync<DownloadRekhtaBookRequest>
    {
        public override async Task<DownloadRekhtaBookRequest> HandleAsync(DownloadRekhtaBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var exporter = new RekhtaDownloader.BookExporter(new ConsoleLogger());
            var path = $"../data/downloads/{Guid.NewGuid():N}";
            try
            {
                var filePath = await exporter.DownloadBook(command.Url, 10, command.CreatePdf ? RekhtaDownloader.OutputType.Pdf : RekhtaDownloader.OutputType.Images, path, cancellationToken);

                var result = new DownloadRekhtaBookRequest.Result()
                {

                    FileName = Path.GetFileName(filePath),
                    MimeType = command.CreatePdf ? "application/pdf" : "application/zip"
                };

                if (command.CreatePdf)
                {
                    result.File = await File.ReadAllBytesAsync(filePath);
                }
                else
                {
                    ZipFile.CreateFromDirectory(filePath, $"{path}.zip");
                    result.File = await File.ReadAllBytesAsync($"{path}.zip");
                }

                command.DownloadResult = result;

                return await base.HandleAsync(command, cancellationToken);
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }
    }
}
