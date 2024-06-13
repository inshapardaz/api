using Inshapardaz.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories;

public interface IFileRepository
{
    Task<FileModel> GetFileById(long id, CancellationToken cancellationToken);

    Task<FileModel> AddFile(FileModel file, CancellationToken cancellationToken);

    Task<FileModel> UpdateFile(FileModel file, CancellationToken cancellationToken);

    Task DeleteFile(long id, CancellationToken cancellationToken);
}
