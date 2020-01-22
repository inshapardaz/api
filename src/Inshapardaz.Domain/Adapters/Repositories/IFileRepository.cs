using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Repositories
{
    public interface IFileRepository
    {
        Task<FileModel> GetFileById(int id, bool isPublic, CancellationToken cancellationToken);

        Task<FileModel> AddFile(FileModel file, string url, bool isPublic, CancellationToken cancellationToken);

        Task<FileModel> UpdateFile(FileModel file, string url, bool isPublic, CancellationToken cancellationToken);

        Task DeleteFile(int id, CancellationToken cancellationToken);
    }
}