using System.IO.Compression;
using System.Text;
using Inshapardaz.Adapters.Dictionary.Formats.Parsers;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Adapters.Dictionary.Formats;

// Unpacks an uploaded dictionary into a temporary directory, picks the parser that recognises the
// files (StarDict first, then DSL) and parses it. Accepts .zip, .tar.gz/.tgz, or a single file.
public class DictionaryFileReader : IReadDictionaryFiles
{
    // Upper bound on unpacked size, so a small compressed upload cannot fill the disk.
    private const long MaxExtractedBytes = 1024L * 1024 * 1024;

    private static readonly IReadOnlyList<IDictionaryParser> Parsers = [new StarDictParser(), new DslParser()];

    public async Task<ParsedDictionaryModel> ReadAsync(Stream file, string fileName, CancellationToken cancellationToken)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "dictimport_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        try
        {
            await ExtractToDirectoryAsync(file, fileName, tempDir, cancellationToken);

            var parser = Parsers.FirstOrDefault(p => p.CanParse(tempDir))
                ?? throw new BadRequestException(
                    "Unable to detect dictionary format. Supported formats: StarDict (.ifo/.idx/.dict) and DSL (.dsl), " +
                    "uploaded as a .zip or .tar.gz archive, or a single .dsl/.dsl.dz file.");

            ParsedDictionary parsed;
            try
            {
                parsed = await parser.ParseAsync(tempDir, cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new BadRequestException(e.Message);
            }

            parsed.Info.OriginalFileName = fileName;
            return new ParsedDictionaryModel { Info = parsed.Info, Articles = parsed.Articles };
        }
        finally
        {
            try { Directory.Delete(tempDir, recursive: true); }
            catch { /* best effort cleanup */ }
        }
    }

    private static async Task ExtractToDirectoryAsync(Stream fileStream, string fileName, string targetDir, CancellationToken ct)
    {
        var lowerName = fileName.ToLowerInvariant();

        if (lowerName.EndsWith(".zip"))
        {
            await ExtractZipAsync(fileStream, targetDir, ct);
        }
        else if (lowerName.EndsWith(".tar.gz") || lowerName.EndsWith(".tgz"))
        {
            await using var gzStream = new GZipStream(fileStream, CompressionMode.Decompress);
            await ExtractTarAsync(gzStream, targetDir, ct);
        }
        else
        {
            var destPath = SafePath(targetDir, Path.GetFileName(fileName))
                ?? throw new BadRequestException("Invalid file name.");
            await using var destStream = File.Create(destPath);
            await CopyLimitedAsync(fileStream, destStream, MaxExtractedBytes, ct);
        }
    }

    private static async Task ExtractZipAsync(Stream stream, string targetDir, CancellationToken ct)
    {
        // ZipArchive needs a seekable stream.
        var buffered = new MemoryStream();
        await stream.CopyToAsync(buffered, ct);
        buffered.Position = 0;

        ZipArchive archive;
        try
        {
            archive = new ZipArchive(buffered, ZipArchiveMode.Read);
        }
        catch (InvalidDataException)
        {
            throw new BadRequestException("The uploaded file is not a valid zip archive.");
        }

        using (archive)
        {
            var remaining = MaxExtractedBytes;
            foreach (var entry in archive.Entries)
            {
                ct.ThrowIfCancellationRequested();
                if (string.IsNullOrEmpty(entry.Name)) continue;

                var destPath = SafePath(targetDir, entry.FullName);
                if (destPath == null) continue;

                Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                await using var entryStream = entry.Open();
                await using var destStream = File.Create(destPath);
                remaining -= await CopyLimitedAsync(entryStream, destStream, remaining, ct);
            }
        }
    }

    // Minimal ustar reader: 512-byte headers, regular files only.
    private static async Task ExtractTarAsync(Stream tarStream, string targetDir, CancellationToken ct)
    {
        var header = new byte[512];
        var remaining = MaxExtractedBytes;

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            if (await ReadExactAsync(tarStream, header, header.Length, ct) < 512) break;
            if (header.All(b => b == 0)) break;

            var nameEnd = Array.IndexOf(header, (byte)0, 0, 100);
            var name = Encoding.UTF8.GetString(header, 0, nameEnd < 0 ? 100 : nameEnd).Trim();
            var sizeText = Encoding.ASCII.GetString(header, 124, 11).Trim('\0', ' ');
            long size;
            try
            {
                size = string.IsNullOrEmpty(sizeText) ? 0 : Convert.ToInt64(sizeText, 8);
            }
            catch (FormatException)
            {
                throw new BadRequestException("The uploaded file is not a valid tar archive.");
            }

            var typeFlag = (char)header[156];
            var padded = size + (size % 512 == 0 ? 0 : 512 - size % 512);
            var destPath = (typeFlag == '0' || typeFlag == '\0') && size > 0 && name.Length > 0 ? SafePath(targetDir, name) : null;

            if (destPath != null)
            {
                if (size > remaining) throw new BadRequestException("The dictionary is too large to import.");
                remaining -= size;

                Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                await using (var destStream = File.Create(destPath))
                {
                    await CopyExactAsync(tarStream, destStream, size, ct);
                }
                await SkipAsync(tarStream, padded - size, ct);
            }
            else
            {
                await SkipAsync(tarStream, padded, ct);
            }
        }
    }

    // Resolves an archive entry inside targetDir; null when it would escape it (path traversal).
    private static string? SafePath(string targetDir, string entryName)
    {
        var root = Path.GetFullPath(targetDir) + Path.DirectorySeparatorChar;
        var destPath = Path.GetFullPath(Path.Combine(targetDir, entryName));
        return destPath.StartsWith(root, StringComparison.OrdinalIgnoreCase) ? destPath : null;
    }

    private static async Task<long> CopyLimitedAsync(Stream source, Stream destination, long limit, CancellationToken ct)
    {
        var buffer = new byte[81920];
        long total = 0;
        int read;
        while ((read = await source.ReadAsync(buffer, ct)) > 0)
        {
            total += read;
            if (total > limit) throw new BadRequestException("The dictionary is too large to import.");
            await destination.WriteAsync(buffer.AsMemory(0, read), ct);
        }
        return total;
    }

    private static async Task CopyExactAsync(Stream source, Stream destination, long count, CancellationToken ct)
    {
        var buffer = new byte[81920];
        while (count > 0)
        {
            var read = await source.ReadAsync(buffer.AsMemory(0, (int)Math.Min(buffer.Length, count)), ct);
            if (read == 0) throw new BadRequestException("The uploaded tar archive is truncated.");
            await destination.WriteAsync(buffer.AsMemory(0, read), ct);
            count -= read;
        }
    }

    private static async Task SkipAsync(Stream stream, long count, CancellationToken ct)
    {
        var buffer = new byte[4096];
        while (count > 0)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(0, (int)Math.Min(buffer.Length, count)), ct);
            if (read == 0) break;
            count -= read;
        }
    }

    private static async Task<int> ReadExactAsync(Stream stream, byte[] buffer, int count, CancellationToken ct)
    {
        var total = 0;
        while (total < count)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(total, count - total), ct);
            if (read == 0) break;
            total += read;
        }
        return total;
    }
}
