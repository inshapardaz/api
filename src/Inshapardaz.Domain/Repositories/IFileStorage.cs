using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories
{
    public interface IFileStorage
    {
        Task<byte[]> GetFile(string filePath, CancellationToken cancellationToken);

        Task<string> StoreFile(string name, byte[] content, CancellationToken cancellationToken);

        Task DeleteFile(string filePath, CancellationToken cancellationToken);

        Task TryDeleteFile(string filePath, CancellationToken cancellationToken);
    }
}