using Inshapardaz.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories
{
    public interface IFileRepository
    {
        Task<FileModel> GetFileById(int id, CancellationToken cancellationToken);

        Task<FileModel> AddFile(FileModel file, CancellationToken cancellationToken);

        Task UpdateFile(FileModel file, CancellationToken cancellationToken);

        Task DeleteFile(int id, CancellationToken cancellationToken);
    }
}
