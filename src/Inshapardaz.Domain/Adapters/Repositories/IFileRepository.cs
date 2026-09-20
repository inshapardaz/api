using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Adapters.Repositories;

public interface IFileRepository
{
    Task<FileModel> GetFileById(long id, CancellationToken cancellationToken);

    Task<FileModel> AddFile(FileModel file, CancellationToken cancellationToken);

    Task<FileModel> UpdateFile(FileModel file, CancellationToken cancellationToken);

    Task DeleteFile(long id, CancellationToken cancellationToken);

    Task UpdateChecksum(long id, string checksum, CancellationToken cancellationToken);

    // Resolves visibility of the book/issue that owns this file (as a page image
    // or content file), so downloads can be gated the same way GetBookContentQuery
    // gates them. Returns null when the file isn't owned by a book or issue at all
    // (author/category/series/bookshelf/library/periodical images are always public),
    // in which case callers should fall back to File.IsPublic.
    Task<bool?> GetFileOwnerIsPublic(long fileId, CancellationToken cancellationToken);
}
