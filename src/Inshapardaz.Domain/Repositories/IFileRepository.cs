using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.Repositories
{
    public interface IFileRepository
    {
        Task<File> GetFileById(int id, CancellationToken cancellationToken);

        Task<File> AddFile(File file, string url, CancellationToken cancellationToken);

        Task<File> UpdateFile(File file, string url, CancellationToken cancellationToken);

        Task DeleteFile(int id, CancellationToken cancellationToken);
    }
}