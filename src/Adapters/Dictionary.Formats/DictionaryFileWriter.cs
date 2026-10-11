using System.IO.Compression;
using Inshapardaz.Adapters.Dictionary.Formats.Export;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Adapters.Dictionary.Formats;

// Exports a dictionary with the matching exporter and zips the files it produces.
public class DictionaryFileWriter : IWriteDictionaryFiles
{
    private static readonly Dictionary<DictionaryExportFormat, IDictionaryExporter> Exporters = new()
    {
        [DictionaryExportFormat.StarDict] = new StarDictExporter(),
        [DictionaryExportFormat.Dsl] = new DslExporter(),
        [DictionaryExportFormat.Json] = new JsonExporter(),
        [DictionaryExportFormat.Kobo] = new KoboExporter(),
        [DictionaryExportFormat.Kindle] = new KindleExporter(),
    };

    public async Task<ExportedFileModel> WriteAsync(DictionaryModel dictionary, IReadOnlyList<DictionaryArticleModel> articles, DictionaryExportFormat format, CancellationToken cancellationToken)
    {
        var exporter = Exporters[format];
        var tempDir = Path.Combine(Path.GetTempPath(), "dictexport_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        try
        {
            var files = await exporter.ExportAsync(dictionary, articles, tempDir, cancellationToken);

            using var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var filePath in files)
                {
                    var entry = archive.CreateEntry(Path.GetFileName(filePath), CompressionLevel.Optimal);
                    await using var entryStream = entry.Open();
                    await using var fileStream = File.OpenRead(filePath);
                    await fileStream.CopyToAsync(entryStream, cancellationToken);
                }
            }

            return new ExportedFileModel
            {
                FileName = $"{SanitizeFileName(dictionary.Name)}_{format.ToString().ToLowerInvariant()}.zip",
                ContentType = "application/zip",
                Data = zipStream.ToArray(),
            };
        }
        finally
        {
            try { Directory.Delete(tempDir, recursive: true); }
            catch { /* best effort cleanup */ }
        }
    }

    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = new string((name ?? "dictionary").Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        return sanitized.Length > 50 ? sanitized[..50] : sanitized;
    }
}
