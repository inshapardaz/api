﻿using Inshapardaz.Domain.Adapters.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Helpers;

public class ImageHelper
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
}
