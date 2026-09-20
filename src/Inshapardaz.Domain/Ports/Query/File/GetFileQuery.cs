using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.Logging;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.File;

public class GetFileQuery(long fileId, int? accountId) : IQuery<FileModel>
{
    public long FileId { get; private set; } = fileId;
    public int? AccountId { get; private set; } = accountId;
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

        // Files that back a book/issue page or content file inherit that book's/issue's
        // own visibility, the same way GetBookContentQuery gates content downloads --
        // File.IsPublic itself isn't a reliable signal for those (page images are saved
        // with IsPublic left at its default regardless of the owning book's visibility).
        // Files with no owning book/issue (author/category/library images, etc.) are
        // always public, so fall back to File.IsPublic for those.
        var ownerIsPublic = await fileRepository.GetFileOwnerIsPublic(query.FileId, cancellationToken);
        var isPublic = ownerIsPublic ?? file.IsPublic;
        if (!isPublic && !query.AccountId.HasValue)
        {
            throw new UnauthorizedException();
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
        {
            file.Contents = stream.ToArray();
        }

        if (string.IsNullOrEmpty(file.Checksum))
        {
            file.Checksum = ChecksumHelper.ComputeChecksum(file.Contents);
            await fileRepository.UpdateChecksum(file.Id, file.Checksum, cancellationToken);
        }

        return file;
    }
}
