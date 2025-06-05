using System;
using Inshapardaz.Domain.Adapters.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Helpers;

public static class ImageHelper
{
    public static async Task<string> TryConvertToPublicFile(long imageId, IFileRepository fileRepository, CancellationToken cancellationToken)
    {
        var image = await fileRepository.GetFileById(imageId, cancellationToken);
        if (image != null && image.IsPublic == true)
        {
            //return image.FilePath.Replace(Settings.BlobRoot, Settings.CDNAddress);
        }

        return null;
    }
    
    public static string GetImageMimeType(this byte[] imageData)
    {
        if (imageData == null || imageData.Length < 4)
            return "application/octet-stream"; // Default unknown binary stream

        // JPEG: FF D8 FF
        if (imageData[0] == 0xFF && imageData[1] == 0xD8 && imageData[2] == 0xFF)
            return "image/jpeg";

        // PNG: 89 50 4E 47 0D 0A 1A 0A
        if (imageData.Length >= 8 &&
            imageData[0] == 0x89 && imageData[1] == 0x50 &&
            imageData[2] == 0x4E && imageData[3] == 0x47 &&
            imageData[4] == 0x0D && imageData[5] == 0x0A &&
            imageData[6] == 0x1A && imageData[7] == 0x0A)
            return "image/png";

        // GIF87a or GIF89a
        if (imageData.Length >= 6 &&
            imageData[0] == 0x47 && imageData[1] == 0x49 &&
            imageData[2] == 0x46 && imageData[3] == 0x38 &&
            (imageData[4] == 0x37 || imageData[4] == 0x39) &&
            imageData[5] == 0x61)
            return "image/gif";

        // BMP: 42 4D
        if (imageData[0] == 0x42 && imageData[1] == 0x4D)
            return "image/bmp";

        // WebP: Starts with RIFF....WEBP
        if (imageData.Length >= 12 &&
            imageData[0] == 0x52 && imageData[1] == 0x49 &&
            imageData[2] == 0x46 && imageData[3] == 0x46 &&
            imageData[8] == 0x57 && imageData[9] == 0x45 &&
            imageData[10] == 0x42 && imageData[11] == 0x50)
            return "image/webp";

        // TIFF (Little Endian): 49 49 2A 00
        if (imageData.Length >= 4 &&
            imageData[0] == 0x49 && imageData[1] == 0x49 &&
            imageData[2] == 0x2A && imageData[3] == 0x00)
            return "image/tiff";

        // TIFF (Big Endian): 4D 4D 00 2A
        if (imageData.Length >= 4 &&
            imageData[0] == 0x4D && imageData[1] == 0x4D &&
            imageData[2] == 0x00 && imageData[3] == 0x2A)
            return "image/tiff";

        return "application/octet-stream";
    }
    
    public static string GetFileExtensionFromMimeType(this string mimeType)
    {
        return mimeType switch
        {
            "image/jpeg" => "jpg",
            "image/png" => "png",
            "image/gif" => "gif",
            "image/bmp" => "bmp",
            "image/webp" => "webp",
            "image/vnd.wap.wbmp" => "wbmp",
            "image/x-pkm" => "pkm",
            "image/ktx" => "ktx",
            "image/heif" => "heif",
            "image/x-icon" => "ico",
            "image/avif" => "avif",
            _ => throw new NotSupportedException($"Mime type '{mimeType}' is not supported.")
        };
    }
}
