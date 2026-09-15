using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.Logging;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.File;

public class GetFileQuery(long fileId) : IQuery<FileModel>
{
    public long FileId { get; private set; } = fileId;
    public int Height { get; set; }
    public int Width { get; set; }
    public bool IsPublic { get; set; }
}

public class GetFileRequestHandler(IFileRepository fileRepository, IFileStorage fileStorage, ILogger<GetFileRequestHandler> logger)
    : QueryHandlerAsync<GetFileQuery, FileModel>
{
    public override async Task<FileModel> ExecuteAsync(GetFileQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var file = await fileRepository.GetFileById(query.FileId, cancellationToken);

        if (file == null)
        {
            // Every failure below collapses into an identical opaque 404 to
            // the client (matches the app's existing NotFoundException
            // contract) -- but these three cases have very different causes,
            // so log which one actually happened for anyone debugging a live
            // "download 404s" report.
            logger.LogWarning("GetFile {FileId}: no File row with this id", query.FileId);
            throw new NotFoundException();
        }

        if (string.IsNullOrWhiteSpace(file.FilePath))
        {
            logger.LogWarning("GetFile {FileId}: File row exists but FilePath is empty", query.FileId);
            throw new NotFoundException();
        }

        var contents = await fileStorage.GetFile(file.FilePath, cancellationToken);
        if (contents == null)
        {
            logger.LogWarning("GetFile {FileId}: FilePath {FilePath} set, but storage backend returned no content for it", query.FileId, file.FilePath);
            throw new NotFoundException();
        }

        using (var stream = new MemoryStream(contents))
        // TODO : Implementation needed
        /*using (var output = new MemoryStream())
        {
            if (IsImageFile(command.Response.MimeType))
            {
                using (Image<Rgba32> image = Image.Load(stream))
                {
                    image.Mutate(x => x.Resize(command.Width, command.Height));
                    image.Save(output, ImageFormats.Jpeg);
                    command.Response.Contents = output.GetBuffer();
                }
            }
            else
            {
                command.Response.Contents = stream.ToArray();
            }
        }*/
        {
            file.Contents = stream.ToArray();
        }

        return file;
    }

    private bool IsImageFile(string mimeType)
    {
        switch (mimeType.ToLower())
        {
            case "image/bmp":
            case "image/jpg":
            case "image/jpeg":
            case "image/png":
            case "image/gif":
                return true;

            default:
                return false;
        }
    }
}
