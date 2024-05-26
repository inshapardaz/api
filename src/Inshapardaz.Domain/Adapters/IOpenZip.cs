using Inshapardaz.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Inshapardaz.Domain.Adapters;

public interface IOpenZip
{
    IEnumerable<FileModel> ExtractImages(byte[] pdfContent);
}

public class ZipReader : IOpenZip
{
    public IEnumerable<FileModel> ExtractImages(byte[] pdfContent)
    {
        var pageImages = new List<FileModel>();

        using (Stream stream = new MemoryStream(pdfContent))
        using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                var extention = Path.GetExtension(entry.FullName);
                if (extention.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                    extention.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    extention.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    var file = entry.Open();
                    byte[] fileContents = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        fileContents = memoryStream.ToArray();
                    }

                    pageImages.Add(new FileModel
                    {
                        FileName = entry.Name,
                        MimeType = extention == ".png" ? "image/png" : "image/jpeg",
                        Contents = fileContents
                    });
                }
            }
        }

        return pageImages;
    }
}
