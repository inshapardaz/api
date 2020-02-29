using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories
{
    public interface IFileStorage
    {
        Task<byte[]> GetFile(string filePath, CancellationToken cancellationToken);

        Task<string> GetTextFile(string filePath, CancellationToken cancellationToken);

        Task<string> StoreImage(string name, byte[] content, CancellationToken cancellationToken);

        Task<string> StoreFile(string name, byte[] content, CancellationToken cancellationToken);

        Task<string> StoreTextFile(string name, string content, CancellationToken cancellationToken);

        Task DeleteImage(string filePath, CancellationToken cancellationToken);

        Task DeleteFile(string filePath, CancellationToken cancellationToken);

        Task TryDeleteFile(string filePath, CancellationToken cancellationToken);

        Task TryDeleteImage(string filePath, CancellationToken cancellationToken);
    }
}
